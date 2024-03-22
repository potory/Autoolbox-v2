using Autoolbox.App.Configuration.Abstraction;
using Autoolbox.App.Exceptions;
using Autoolbox.App.Overrides;
using Autoolbox.App.Services.Abstraction;
using Autoolbox.App.Services.Implementation;
using Newtonsoft.Json.Linq;
using SonScript2.Core.Compilation.Abstraction;
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

    public RunCommand(IConfigReader configReader, ICompiler compiler,
        IRequestFactory requestFactory, IRequestSender requestSender)
    {
        _configReader = configReader;
        _compiler = compiler;
        _requestFactory = requestFactory;
        _requestSender = requestSender;
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var resultCache = new Dictionary<int, string>();
        
        var segments = GetConfigSegments(settings.ConfigPath);
        var sequences = GetConfigSequences(segments);

        int level = 0;

        foreach (var sequence in sequences)
        {
            var sequenceContext = GetSequenceContext(sequence);
            var directory = HandleDirectory(settings.OutputPath, level);

            sequenceContext.Level = level;
            
            for (int index = 0; index < settings.Count; index++)
            {
                sequenceContext.Iteration = index;

                var previousResult = GetPreviousResult(resultCache, index);
                
                var requestOption = await _requestFactory.CreateAsync(sequence, previousResult);
                if (!requestOption.IsValid)
                {
                    continue;
                }
                
                var result = await SendRequest(requestOption);
                var path = await SaveResult(directory, $"{index:0000}.png", result);

                resultCache[index] = path;
            }

            level++;
        }
        
        return 0;
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