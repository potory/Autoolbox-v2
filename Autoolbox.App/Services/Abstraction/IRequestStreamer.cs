using Autoolbox.App.Services.Implementation;
using Newtonsoft.Json.Linq;

namespace Autoolbox.App.Services.Abstraction;

public interface IRequestStreamer
{
    public Task<RequestOption> StreamToMemoryAsync(JObject request, string? imagePath);
}