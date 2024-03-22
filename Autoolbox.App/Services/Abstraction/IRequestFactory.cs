using Autoolbox.App.Services.Implementation;
using SonScript2.Core.Compilation.Abstraction;

namespace Autoolbox.App.Services.Abstraction;

public interface IRequestFactory
{
    public Task<RequestOption> CreateAsync(INodeSequence sequence, string? previousResultPath = null);
}