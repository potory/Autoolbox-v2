using Newtonsoft.Json.Linq;

namespace Autoolbox.App.Services.Abstraction;

public interface IRequestStreamer
{
    public Task<Stream> StreamToMemoryAsync(JToken request);
    public Task<Stream> StreamToMemoryAsync(JToken request, string imagePath);
}