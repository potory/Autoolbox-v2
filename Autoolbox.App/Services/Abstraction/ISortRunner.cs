namespace Autoolbox.App.Services.Abstraction;

public interface ISortRunner
{
    public Task<bool> Run(string directory, string outputPath);
}