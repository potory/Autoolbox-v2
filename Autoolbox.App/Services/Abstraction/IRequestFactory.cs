using SonScript2.Core.Compilation.Abstraction;

namespace Autoolbox.App.Services.Abstraction;

public interface IRequestFactory
{
    public Task<Stream> Create(INodeSequence sequence);
}