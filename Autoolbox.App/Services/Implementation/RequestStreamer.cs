using Autoolbox.App.Exceptions;
using Autoolbox.App.Services.Abstraction;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Autoolbox.App.Services.Implementation;

public class RequestStreamer : IRequestStreamer
{
    private readonly string _injection;

    public RequestStreamer(IConfiguration configuration)
    {
        _injection = configuration[Contracts.Config.Injection.PreviousResult]!;
        
        if (string.IsNullOrEmpty(_injection))
        {
            throw new AutoolboxConfigurationException("...");
        }
    }
    
    public async Task<Stream> StreamToMemoryAsync(JToken request)
    {
        var stream = new MemoryStream();
        var streamWriter = new StreamWriter(stream);

        await streamWriter.WriteAsync(request.ToString());
        return await FinishWrite(streamWriter, stream);
    }

    public async Task<Stream> StreamToMemoryAsync(JToken request, string imagePath)
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

            segmentStart = injectionIndex + _injection.Length;
            injectionIndex = json.IndexOf(_injection, segmentStart, StringComparison.Ordinal);
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

    private int FindInjectionIndex(string content, int start) => content.IndexOf(_injection, start, StringComparison.Ordinal);
}