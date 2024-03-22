using Autoolbox.App.Configuration.Abstraction;
using Autoolbox.App.Exceptions;
using Autoolbox.App.Overrides;
using Autoolbox.App.Services.Abstraction;
using Autoolbox.App.Services.Implementation;
using Autoolbox.App.Utility;
using Newtonsoft.Json.Linq;
using SonScript2.Core.Compilation.Abstraction;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Autoolbox.App.Commands;

public sealed class RunCommand : AsyncCommand<RunCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<Config>")] public required string ConfigPath { get; set; }
        [CommandArgument(1, "<Output>")] public required string OutputPath { get; set; }
        [CommandArgument(2, "[Count]")] public int Count { get; set; }
    }

    private readonly IConfigReader _configReader;
    private readonly ICompiler _compiler;
    private readonly IRequestFactory _requestFactory;
    private readonly IRequestSender _requestSender;

    private readonly Dictionary<int, string> _resultCache = new();

    public RunCommand(IConfigReader configReader, ICompiler compiler,
        IRequestFactory requestFactory, IRequestSender requestSender)
    {
        _configReader = configReader;
        _compiler = compiler;
        _requestFactory = requestFactory;
        _requestSender = requestSender;
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
            var sequence = sequences[level];

            var context = GetSequenceContext(sequence);
            var directory = HandleDirectory(settings.OutputPath, level);
            context.Level = level;
            
            for (var i = 0; i < settings.Count; i++)
            {
                context.Iteration = i;
                await GenerateFromSequence(sequence, context, directory);
                progressTask.Increment(deltaProgress);
            }
            
            progressTask.StopTask();
        });
        
        return 0;
    }

    private async Task GenerateFromSequence(INodeSequence sequence, AutomaticContext context, string directory)
    {
        var previousResult = GetPreviousResult(_resultCache, context.Iteration);
        var requestOption = await _requestFactory.CreateAsync(sequence, previousResult);

        if (!requestOption.IsValid)
        {
            return;
        }

        var result = await SendRequest(requestOption);
        var path = await SaveResult(directory, $"{context.Iteration:0000}.png", result);

        _resultCache[context.Iteration] = path;
    }

    private static string GetPreviousResult(IReadOnlyDictionary<int, string> cache, int index)
    {
        if (cache.TryGetValue(index, out var value))
            return value;
        
        return string.Empty;
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