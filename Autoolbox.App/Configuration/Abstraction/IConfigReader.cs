namespace Autoolbox.App.Configuration.Abstraction;

public interface IConfigReader
{
    public string ReadInitFunction(string source);
    public IEnumerable<string> Read(string source);
}