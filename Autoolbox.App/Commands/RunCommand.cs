using Autoolbox.App.Configuration.Abstraction;
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

    public RunCommand(IConfigReader configReader, ICompiler compiler)
    {
        _configReader = configReader;
        _compiler = compiler;
    }
    
    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        return Task.FromResult(0);
    }
}