using System.Diagnostics;
using Autoolbox.App.Services.Abstraction;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace Autoolbox.App.Services.Implementation;

public class SortRunner : ISortRunner
{
    private readonly string? _sortingApplicationPath;

    public SortRunner(IConfiguration config)
    {
        _sortingApplicationPath = config[Contracts.Config.ExternalDependencies.SortingApplicationPath];
    }

    public async Task<bool> Run(string directory, string outputPath)
    {
        if (string.IsNullOrEmpty(_sortingApplicationPath))
        {
            AnsiConsole.WriteLine("WARNING: externalDependencies:sortingApplicationPath is not setted in appconfig.json to run sorting");
            return false;
        }
        
        directory = Path.GetFullPath(directory);
        outputPath = Path.Combine(Path.GetFullPath(outputPath), "_excluded", new DirectoryInfo(directory).Name);
        
        var p = new Process();
        p.StartInfo.FileName = _sortingApplicationPath;
        p.StartInfo.Arguments = $"{directory} {outputPath}";
        p.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
        p.Start();

        await p.WaitForExitAsync();
        return true;
    }
}