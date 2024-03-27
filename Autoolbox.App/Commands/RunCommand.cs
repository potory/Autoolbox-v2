using Autoolbox.App.Configuration.Abstraction;
using Autoolbox.App.Exceptions;
using Autoolbox.App.Overrides;
using Autoolbox.App.Services.Abstraction;
using Autoolbox.App.Services.Implementation;
using Autoolbox.App.Utility;
using Autoolbox.App.Wrappers;
using Newtonsoft.Json.Linq;
using SonScript2.Core.Compilation.Abstraction;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Autoolbox.App.Commands;

public record CachedProgress(JObject? PreviousRequest, string PreviousResult);

public sealed class RunCommand : AsyncCommand<RunCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<Config>")] public required string ConfigPath { get; set; }
        [CommandArgument(1, "<Output>")] public required string OutputPath { get; set; }
        [CommandArgument(2, "[Count]")] public int Count { get; set; }
        [CommandOption("--pausing")] public bool? Pausing { get; set; }
        [CommandOption("--autorunSort")] public bool? AutorunSort { get; set; }
    }

    private readonly IConfigReader _configReader;
    private readonly ICompiler _compiler;
    private readonly IRequestFactory _requestFactory;
    private readonly IRequestSender _requestSender;
    private readonly ISortRunner _sortRunner;

    private readonly Dictionary<int, CachedProgress> _cache = new();

    public RunCommand(IConfigReader configReader, ICompiler compiler,
        IRequestFactory requestFactory, IRequestSender requestSender, ISortRunner sortRunner)
    {
        _configReader = configReader;
        _compiler = compiler;
        _requestFactory = requestFactory;
        _requestSender = requestSender;
        _sortRunner = sortRunner;
    }
    
    public override async Task<int> ExecuteAsync(CommandContext commandContext, Settings settings)
    {
        var deltaProgress = 1 / (float)settings.Count * 100;

        AnsiConsole.WriteLine("Getting config segments...");
        var segments = GetConfigSegments(settings.ConfigPath);
        AnsiConsole.WriteLine("Getting config sequences...");
        var sequences = GetConfigSequences(segments).ToArray();

        AnsiConsole.WriteLine("Generation started...");
        await LeveledProgressHandler.Handle(sequences.Length, async (level, progressTask) =>
        {
            try
            {
                var sequence = sequences[level];

                var context = GetSequenceContext(sequence);
                var directory = HandleDirectory(settings.OutputPath, level);
                context.Level = level;
            
                for (var i = 0; i < settings.Count; i++)
                {
                    var requestWrapper = TryGetPreviousRequest(i, out var request) ? 
                        new RequestWrapper(request!) : 
                        RequestWrapper.EmptyRequest;

                    context.PreviousRequest = requestWrapper;
                    context.Iteration = i;
                    
                    await GenerateFromSequence(sequence, requestWrapper, context, directory);
                    progressTask.Increment(deltaProgress);
                }
            
                progressTask.StopTask();

                if (settings.Pausing.HasValue && 
                    settings.Pausing.Value &&
                    level != sequences.Length - 1)
                {

                    if (!settings.AutorunSort.HasValue || !settings.AutorunSort.Value ||
                        !await _sortRunner.Run(directory, settings.OutputPath))
                    {
                        AnsiConsole.WriteLine("Press any key to move to next level");
                        Console.ReadKey();
                    }
                }
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
                throw;
            }
        });

        return 0;
    }

    private async Task GenerateFromSequence(INodeSequence sequence, RequestWrapper previousRequestWrapper, AutomaticContext context, string directory)
    {
        var previousResult = GetPreviousResult(context.Iteration);
        var requestOption = await _requestFactory.CreateAsync(sequence, previousRequestWrapper, previousResult);

        if (!requestOption.IsValid)
        {
            return;
        }

        var result = await SendRequest(requestOption);
        var path = await SaveResult(directory, $"{context.Iteration:0000}.png", result);

        _cache[context.Iteration] = new CachedProgress(requestOption.RequestJson!, path);
    }

    private bool TryGetPreviousRequest(int index, out JObject? request)
    {
        if(_cache.TryGetValue(index, out var value))
        {
            request = value.PreviousRequest;
            return true;
        }

        request = null;
        return false;
    }

    private string GetPreviousResult(int index)
    {
        return _cache.TryGetValue(index, out var value) ? 
            value.PreviousResult : 
            string.Empty;
    }

    private static async Task<string> SaveResult(string directory, string filename, string result)
    {
        var filePath = Path.Combine(directory, filename);
        await File.WriteAllBytesAsync(filePath, Convert.FromBase64String(result));

        return filePath;
    }

    private Task<string> SendRequest(RequestOption requestOption) => 
        _requestSender.SendRequestAsync(requestOption.RequestStream!, requestOption.RequestType);

    private IEnumerable<INodeSequence> GetConfigSequences(IEnumerable<string> segments)
    {
        foreach (var segment in segments)
        {
            var sequence = _compiler.Compile(segment);

            if (sequence == null)
            {
                throw new AutoolboxConfigurationException("...");
            }

            yield return sequence;
        }
    }

    private static AutomaticContext GetSequenceContext(INodeSequence sequence) => 
        sequence.Context as AutomaticContext ?? throw new Exception();

    private IEnumerable<string> GetConfigSegments(string configPath)
    {
        var configContent = File.ReadAllText(configPath);
        return _configReader.Read(configContent);
    }

    private static string HandleDirectory(string outputPath, int currentStep)
    {
        var directory = Path.Combine(outputPath, $"{currentStep}");
        Directory.CreateDirectory(directory);
        return directory;
    }
}