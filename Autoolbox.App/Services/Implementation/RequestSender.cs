using Autoolbox.App.Exceptions;
using Autoolbox.App.Services.Abstraction;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Autoolbox.App.Services.Implementation;

public class RequestSender : IRequestSender
{
    private readonly string _textToImgEndpoint;
    private readonly string _imgToImgEndpoint;
    
    private readonly HttpClient _client = new();

    public RequestSender(IConfiguration configuration)
    {
        _textToImgEndpoint = configuration["endpoints:txt2img"] ?? throw new AutoolboxConfigurationException("...");
        _imgToImgEndpoint = configuration["endpoints:img2img"] ?? throw new AutoolboxConfigurationException("...");

        _client.Timeout = TimeSpan.FromMinutes(5);
    }

    public async Task<string> SendRequestAsync(Stream stream, RequestType type)
    {
        return type switch
        {
            RequestType.TextToImage => await SendRequestAsync(_textToImgEndpoint, stream),
            RequestType.ImageToImage => await SendRequestAsync(_imgToImgEndpoint, stream),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private async Task<string> SendRequestAsync(string address, Stream contentStream)
    {
        var content = new StreamContent(contentStream);
        var resultContent = _client.PostAsync(address, content).Result.Content;
        var stream = await resultContent.ReadAsStreamAsync();

        using var sr = new StreamReader(stream);
        var response = await sr.ReadToEndAsync();

        var imageContent = JObject.Parse(response)["images"]![0]!.ToString();

        return imageContent;
    }
}