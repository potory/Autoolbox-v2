namespace Autoolbox.App.Services.Abstraction;

public enum RequestType
{
    TextToImage,
    ImageToImage
}

public interface IRequestSender
{
    public Task<string> SendRequestAsync(Stream stream, RequestType type);
}