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

        return configurator;
    }
}