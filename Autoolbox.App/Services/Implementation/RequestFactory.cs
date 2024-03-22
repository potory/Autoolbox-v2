using System.Text;
using Autoolbox.App.Exceptions;
using Autoolbox.App.Extensions;
using Autoolbox.App.Services.Abstraction;
using LanguageExt.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SonScript2.Core.Compilation.Abstraction;

namespace Autoolbox.App.Services.Implementation;

public class RequestFactory : IRequestFactory
{
    private readonly ILogger _logger;
    private readonly IRequestStreamer _requestStreamer;
    private readonly StringBuilder _stringBuilder = new();
    private readonly JObject _defaultRequest;

    public RequestFactory(IConfiguration config, ILogger logger, IRequestStreamer requestStreamer)
    {
        _logger = logger;
        _requestStreamer = requestStreamer;
        var configPath = config[Contracts.Config.Defaults.Request];

        if (string.IsNullOrEmpty(configPath))
        {
            throw new AutoolboxConfigurationException("...");
        }
        
        var content = File.ReadAllText(configPath);
        _defaultRequest = JObject.Parse(content);
    }

    public async Task<RequestOption> CreateAsync(INodeSequence sequence, string? previousResultPath = null)
    {
        var concreteRequest = await GetConcreteRequest(sequence);
        var mergedRequest = _defaultRequest.MergeWith(concreteRequest);

        return await _requestStreamer.StreamToMemoryAsync(mergedRequest, previousResultPath);
    }

    private async Task<JObject> GetConcreteRequest(INodeSequence sequence)
    {
        try
        {
            await sequence.Evaluate(_stringBuilder);
        }
        catch (Exception e)
        {
            _logger.LogException(e);
            throw;
        }

        var concreteRequest = JObject.Parse(_stringBuilder.ToString());
        _stringBuilder.Clear();
        
        return concreteRequest;
    }
}