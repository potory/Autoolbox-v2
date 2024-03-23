using Autoolbox.App.Services.Implementation;
using Autoolbox.App.Wrappers;
using SonScript2.Core.Compilation.Abstraction;

namespace Autoolbox.App.Services.Abstraction;

public interface IRequestFactory
{
    public Task<RequestOption> CreateAsync(INodeSequence sequence, RequestWrapper previousRequest, string? previousResultPath = null);
}