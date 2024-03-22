using Autoolbox.App.Services.Abstraction;

namespace Autoolbox.App.Services.Implementation;

public class RequestOption
{
    private readonly Exception? _exception;

    public bool IsValid => _exception == null && RequestStream != null;

    public RequestType RequestType { get; }
    public Stream? RequestStream { get; }


    public RequestOption(Stream stream, RequestType type)
    {
        RequestStream = stream;
        RequestType = type;
    }

    public RequestOption(Exception exception)
    {
        _exception = exception;
    }
}