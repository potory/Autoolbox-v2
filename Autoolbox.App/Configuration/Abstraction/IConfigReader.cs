namespace Autoolbox.App.Configuration.Abstraction;

public interface IConfigReader
{
    public IEnumerable<string> Read(string source);
}