using Newtonsoft.Json;
using Spectre.Console.Cli;

namespace Autoolbox.App.Commands;

public class JsonToPlainCommand : AsyncCommand<JsonToPlainCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<Input>")] public required string InputPath { get; set; }
        [CommandArgument(1, "<Output>")] public required string OutputPath { get; set; }
        [CommandArgument(2, "[Extension]")] public string Extension { get; set; } = "txt";
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var content = await File.ReadAllTextAsync(settings.InputPath);
        var arr = JsonConvert.DeserializeObject<string[]>(content);

        if (arr is null)
        {
            return -1;
        }
        
        File.WriteAllLines(settings.OutputPath, arr);
        return 0;
    }
}