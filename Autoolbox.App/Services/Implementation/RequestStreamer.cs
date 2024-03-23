using Autoolbox.App.Exceptions;
using Autoolbox.App.Services.Abstraction;
using LanguageExt.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Autoolbox.App.Services.Implementation;

public class RequestStreamer : IRequestStreamer
{
    private readonly string _injectionMarker;

    public RequestStreamer(IConfiguration configuration)
    {
        _injectionMarker = configuration[Contracts.Config.Injection.PreviousResult]!;
        
        if (string.IsNullOrEmpty(_injectionMarker))
        {
            throw new AutoolboxConfigurationException("...");
        }
    }

    public async Task<RequestOption> StreamToMemoryAsync(JObject request, string? imagePath)
    {
        if (!RequireInjection(request))
        {
            return new RequestOption(request, await StreamToMemoryAsync(request), RequestType.TextToImage);
        }

        if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
        {
            return new RequestOption(new AutomaticExecutionException("..."));
        }
            
        var stream = await StreamToMemoryWithInjectionAsync(request, imagePath);
        return new RequestOption(request, stream, RequestType.ImageToImage);
    }

    private async Task<Stream> StreamToMemoryAsync(JObject request)
    {
        var stream = new MemoryStream();
        var streamWriter = new StreamWriter(stream);

        await streamWriter.WriteAsync(request.ToString());
        return await FinishWrite(streamWriter, stream);
    }

    private async Task<Stream> StreamToMemoryWithInjectionAsync(JToken request, string imagePath)
    {
        var stream = new MemoryStream();
        var streamWriter = new StreamWriter(stream);

        string json = request.ToString();

        int segmentStart = 0;
        int injectionIndex = FindInjectionIndex(json, segmentStart);

        if (injectionIndex == -1)
            throw new ArgumentException();

        var imageString = await ReadImageAsBase64String(imagePath);

        while (injectionIndex > -1)
        {
            var segmentLength = injectionIndex - segmentStart;
            streamWriter.Write(json.AsSpan(segmentStart, segmentLength));
            await streamWriter.WriteAsync(imageString);

            segmentStart = injectionIndex + _injectionMarker.Length;
            injectionIndex = json.IndexOf(_injectionMarker, segmentStart, StringComparison.Ordinal);
        }

        streamWriter.Write(json.AsSpan(segmentStart, json.Length - segmentStart));

        return await FinishWrite(streamWriter, stream);
    }

    private static async Task<T> FinishWrite<T>(TextWriter streamWriter, T stream) where T: Stream
    {
        await streamWriter.FlushAsync();
        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }

    private static async Task<string> ReadImageAsBase64String(string previousImagePath) => Convert.ToBase64String(await File.ReadAllBytesAsync(previousImagePath));

    private int FindInjectionIndex(string content, int start) => content.IndexOf(_injectionMarker, start, StringComparison.Ordinal);
    
    private bool RequireInjection(JContainer container)
    {
        return container
            .Descendants()
            .Any(token => token is JValue { Value: string str } && str == _injectionMarker);
    }
}