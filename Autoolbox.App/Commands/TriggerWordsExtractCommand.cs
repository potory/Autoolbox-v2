using Spectre.Console.Cli;

namespace Autoolbox.App.Commands;

public class TriggerWordsExtractCommand : AsyncCommand<TriggerWordsExtractCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<Input>")] public required string InputPath { get; set; }
        [CommandArgument(1, "<Output>")] public required string OutputPath { get; set; }
        [CommandArgument(2, "[Extension]")] public string Extension { get; set; } = "txt";
    }

    
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var extension = settings.Extension.Trim().Trim('.');
        var captionFiles = Directory.GetFiles(settings.InputPath, $"*.{extension}");

        var triggerWords = new HashSet<string>();

        foreach (var captionFile in captionFiles)
        {
            var content = await File.ReadAllTextAsync(captionFile);
            var words = content.Split(',');

            foreach (var word in words)
            {
                triggerWords.Add(word.Trim());
            }
        }
        
        await File.WriteAllTextAsync(settings.OutputPath, string.Join(Environment.NewLine, triggerWords));
        return 0;
    }
}