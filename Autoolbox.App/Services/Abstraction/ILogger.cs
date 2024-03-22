namespace Autoolbox.App.Services.Abstraction;

public interface ILogger
{
    public void LogException(Exception exception);
}