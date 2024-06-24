using Shared.Wrapper;
using Polly.Registry;
using Domain.Config.Genesys;
using Microsoft.Extensions.Options;
using Application.Common.Interfaces;
using PureCloudPlatform.Client.V2.Client;
using Application.Common.ExceptionHandlers;
using Application.Common.Interfaces.Genesys;

namespace Infrastructure.Genesys.Client;

public class GenesysApiClient(
    IOptions<GenesysClientConfig> genesysConfig,
    IGenesysAuthHandler genesysAuthHandler,
    ResiliencePipelineProvider<string> pipelineProvider)
    : IGenesysApiClient
{
    private Configuration Configuration { get; set; } = new();
    private readonly GenesysClientConfig _genesysConfig = genesysConfig.Value;
    private readonly ResiliencePipelineProvider<string> _pipelineProvider = pipelineProvider;
    private readonly IGenesysAuthHandler _genesysAuthHandler = genesysAuthHandler;

    public async Task AuthenticateAsync()
    {
        try
        {
            var region = GenesysHelper.GetRegion(_genesysConfig.Region ?? string.Empty);
            Configuration.ApiClient.setBasePath(region);

            var tokenInfo = await _genesysAuthHandler.GetAccessTokenInfoAsync(
                _genesysConfig?.ClientId,
                _genesysConfig?.ClientSecret,
                Configuration.ApiClient);

            if (!tokenInfo.IsFailure)
                Configuration.AccessToken = tokenInfo.Data.AccessToken;
            else
                throw new Exception($"code: {tokenInfo.Error?.Code}, Error: {tokenInfo.Error?.Message}");
        }
        catch (Exception ex)
        {
            throw ex.With(ex.Message, ex.Message);
        }
    }

    public T CreateApiInstance<T>() where T : class
    {
        try
        {
            var constructor = typeof(T).GetConstructor(new[] { typeof(Configuration) });
            if (constructor == null)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have a constructor with a Configuration parameter.");
            }

            return (T)constructor.Invoke(new object[] { Configuration });
        }
        catch (Exception ex)
        {
            throw ex.With(ex.Message, ex.Source, ex.StackTrace)
                .DetailData(typeof(T).Name.ToString(), typeof(T).Name);
        }
    }

    public async Task<TResult> ExecuteWithRetryAsync<TResult>(Func<Task<TResult>> action)
    {
        await AuthenticateAsync();
        var pipeline = _pipelineProvider.GetPipeline("default");

        try
        {
            var result = await pipeline.ExecuteAsync(async ct => await action());
            return result;
        }
        catch (Exception ex)
        {
            throw ex.With(ex.Source, ex.Message);
        }
    }
    public TResult ExecuteWithRetry<TResult>(Func<TResult> action)
    {
        AuthenticateAsync().GetAwaiter().GetResult();
        var pipeline = _pipelineProvider.GetPipeline("default");

        try
        {
            var result = pipeline.Execute(ct => action());
            return result;
        }
        catch (Exception ex)
        {
            throw ex.With(ex.Source, ex.Message);
        }
    }

    public async Task<Result<T>> ExecuteWithRetryAsync<T>(Func<Task<Result<T>>> action)
    {
        return await ExecuteWithRetryAsync(action);
    }

    public Result<T> ExecuteWithRetry<T>(Func<Result<T>> action)
    {
        return ExecuteWithRetry(action);
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action)
    {
        await AuthenticateAsync();
        try
        {
            var result = await action();
            return result;
        }
        catch (Exception ex)
        {
            throw ex.With(ex.Source, ex.Message);
        }
    }

    public TResult Execute<TResult>(Func<TResult> action)
    {
        try
        {
            var result = action();
            return result;
        }
        catch (Exception ex)
        {
            throw ex.With(ex.Source, ex.Message);
        }
    }
}