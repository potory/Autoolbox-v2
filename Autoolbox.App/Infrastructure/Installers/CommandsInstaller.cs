using Spectre.Console.Cli;

namespace Autoolbox.App.Infrastructure.Installers;

public static class CommandsInstaller
{
    public static IConfigurator AddSonScriptCommands(this IConfigurator configurator)
    {
        return configurator;
    }
}