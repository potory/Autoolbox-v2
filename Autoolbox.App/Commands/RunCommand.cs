using Autoolbox.App.Configuration.Abstraction;
using Autoolbox.App.Exceptions;
using Autoolbox.App.Services.Abstraction;
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

    public RunCommand(IConfigReader configReader, ICompiler compiler, IRequestFactory requestFactory)
    {
        _configReader = configReader;
        _compiler = compiler;
        _requestFactory = requestFactory;
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var segments = GetConfigSegments(settings.ConfigPath);
        var sequences = GetConfigSequences(segments);
        
        return 0;
    }

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

    private IEnumerable<string> GetConfigSegments(string configPath)
    {
        var configContent = File.ReadAllText(configPath);
        return _configReader.Read(configContent);
    }
}