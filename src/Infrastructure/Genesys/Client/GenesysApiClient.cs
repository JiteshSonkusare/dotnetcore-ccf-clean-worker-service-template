using Polly.Registry;
using Application.Common.Interfaces;
using PureCloudPlatform.Client.V2.Client;
using Application.Common.Interfaces.Genesys;

namespace Infrastructure.Genesys.Client;

public class GenesysApiClient(
    IGenesysAuthHandler genesysAuthHandler,
    ResiliencePipelineProvider<string> pipelineProvider)
    : IGenesysApiClient
{
    private Configuration Configuration { get; set; } = new();
    private readonly IGenesysAuthHandler _genesysAuthHandler = genesysAuthHandler;
    private readonly ResiliencePipelineProvider<string> _pipelineProvider = pipelineProvider;

    public T CreateApiInstance<T>() where T : class
    {
        var constructor = typeof(T).GetConstructor(new[] { typeof(Configuration) });

        if (constructor == null)
            throw new InvalidOperationException($"Type {typeof(T).Name} does not have a constructor with a Configuration parameter.");

        return (T)constructor.Invoke(new object[] { Configuration });
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action, bool useRetry = false)
    {
        await _genesysAuthHandler.AuthenticateAsync(Configuration);
        if (useRetry)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            return await pipeline.ExecuteAsync(async ct => await action());
        }
        else
        {
            return await action();
        }
    }

    public TResult Execute<TResult>(Func<TResult> action, bool useRetry = false)
    {
        _genesysAuthHandler.AuthenticateAsync(Configuration).GetAwaiter().GetResult();
        if (useRetry)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            return pipeline.Execute(ct => action());
        }
        else
        {
            return action();
        }
    }
}