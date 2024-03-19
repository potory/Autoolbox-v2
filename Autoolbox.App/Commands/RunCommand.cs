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

    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        AnsiConsole.WriteLine("Hello, Run!");
        return Task.FromResult(0);
    }
}