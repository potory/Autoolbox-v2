using Autoolbox.App.Commands;
using Spectre.Console.Cli;

namespace Autoolbox.App.Infrastructure.Installers;

public static class CommandsInstaller
{
    public static IConfigurator AddSonScriptCommands(this IConfigurator configurator)
    {
        configurator.AddCommand<RunCommand>("run")
            .WithAlias("r")
            .WithDescription("Run image generation based on *.sonscript config file")
            .WithExample("run", "config.sonscript", "images/output", "10");
        
        configurator.AddCommand<TriggerWordsExtractCommand>("extractTags")
            .WithAlias("et")
            .WithDescription("Run image generation based on *.sonscript config file")
            .WithExample("extractTags", "LoRA/Datasets/exampleSet", "LoRA/TriggerWords/example.txt", "txt");
        
        configurator.AddCommand<JsonToPlainCommand>("jsontoplain")
            .WithAlias("jstp")
            .WithDescription("Run image generation based on *.sonscript config file")
            .WithExample("extractTags", "LoRA/Datasets/exampleSet", "LoRA/TriggerWords/example.txt", "txt");

        return configurator;
    }
}