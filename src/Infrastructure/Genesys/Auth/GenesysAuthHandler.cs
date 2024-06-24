using Shared.Wrapper;
using Domain.Models.Genesys;
using Domain.Config.Genesys;
using Microsoft.Extensions.Options;
using static Domain.Errors.ErrorDto;
using PureCloudPlatform.Client.V2.Client;
using Application.Common.ExceptionHandlers;
using Application.Common.Interfaces.Genesys;
using PureCloudPlatform.Client.V2.Extensions;

namespace Infrastructure.Genesys.Auth;

public class GenesysAuthHandler(
    IOptions<GenesysClientConfig> genesysConfig)
    : IGenesysAuthHandler
{
    private readonly GenesysClientConfig _genesysConfig = genesysConfig.Value;

    public async Task AuthenticateAsync(Configuration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var region = GenesysHelper.GetRegion(_genesysConfig.Region);
        configuration.ApiClient.setBasePath(region);

        var tokenInfo = await GetAccessTokenInfoAsync(configuration.ApiClient);

        if (tokenInfo.Suceeded)
            configuration.AccessToken = tokenInfo.Data.AccessToken;
        else
            throw new Exception($"code: {tokenInfo.Error?.Code}, Error: {tokenInfo.Error?.Message}");
    }

    private async Task<Result<AuthToken>> GetAccessTokenInfoAsync(ApiClient apiClient)
    {
        ArgumentNullException.ThrowIfNull(apiClient);

        try
        {
            var tokenInfo = await Task.Run(() =>
                    apiClient.PostToken(
                        _genesysConfig.ClientId,
                        _genesysConfig.ClientSecret))
                    .ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(tokenInfo.AccessToken))
            {
                var authTokenInfo = new AuthToken(
                    tokenInfo.AccessToken,
                    tokenInfo.RefreshToken,
                    tokenInfo.TokenType,
                    tokenInfo.ExpiresIn,
                    tokenInfo.Error);
                return Result.Success(authTokenInfo);
            }

            return Result.Failure<AuthToken>(
                AuthErrors.AuthTokenError(
                    tokenInfo.Error?.ToString() ?? string.Empty));
        }
        catch (Exception ex)
        {
            throw ex.With(ex.Source, ex.Message);
        }
    }
}