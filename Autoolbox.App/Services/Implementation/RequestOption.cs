using Autoolbox.App.Services.Abstraction;
using Newtonsoft.Json.Linq;

namespace Autoolbox.App.Services.Implementation;

public class RequestOption
{
    private readonly Exception? _exception;

    public bool IsValid => _exception == null && RequestStream != null;

    public RequestType RequestType { get; }
    public Stream? RequestStream { get; }
    public JObject? RequestJson { get; }


    public RequestOption(JObject json, Stream stream, RequestType type)
    {
        RequestJson = json;
        RequestStream = stream;
        RequestType = type;
    }

    public RequestOption(Exception exception)
    {
        _exception = exception;
    }
}