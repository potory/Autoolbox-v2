using Autoolbox.App.Exceptions;
using Autoolbox.App.Services.Abstraction;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace Autoolbox.App.Services.Implementation;

public class Logger : ILogger
{
    private readonly string _path;

    public Logger(IConfiguration configuration)
    {
        _path = configuration[Contracts.Config.Directories.Logs]!;

        if (string.IsNullOrEmpty(_path))
        {
            throw new AutoolboxConfigurationException("...");
        }

        Directory.CreateDirectory(_path);
    }

    public void LogException(Exception exception)
    {
        var timestamp = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
        File.WriteAllText( $"{_path}/{timestamp}.txt", exception.ToString());
        
        AnsiConsole.WriteException(exception);
        AnsiConsole.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}