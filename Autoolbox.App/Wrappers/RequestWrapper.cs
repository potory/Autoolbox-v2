using Autoolbox.App.Exceptions;
using Newtonsoft.Json.Linq;

namespace Autoolbox.App.Wrappers;

public class RequestWrapper
{
    public static RequestWrapper EmptyRequest { get; } = new(JObject.Parse("{}"));

    public JObject Json { get; }

    public RequestWrapper(JObject json)
    {
        Json = json;
    }

    public T Value<T>(string path)
    {
        var token = Json[path];

        if (token == null)
        {
            throw new AutomaticExecutionException("...");
        }

        var value = token.Value<T>();
        
        if (value == null)
        {
            throw new AutomaticExecutionException("...");
        }
        
        return value;
    }
}