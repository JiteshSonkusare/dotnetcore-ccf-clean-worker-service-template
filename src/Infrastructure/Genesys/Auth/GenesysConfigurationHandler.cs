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

public class GenesysConfigurationHandler(
    IOptions<GenesysConfig> genesysConfig)
    : IGenesysConfigurationHandler
{
    private readonly GenesysConfig _genesysConfig = genesysConfig.Value;

	public async Task InitializeConfigurationAsync(Configuration configuration, bool useRetry = false)
	{
		ArgumentNullException.ThrowIfNull(configuration);

		var region = GenesysHelper.GetRegion(_genesysConfig.ClientDetails.Region);
		configuration.ApiClient.setBasePath(region);

		if (useRetry)
			GenesysHelper.UseRetry(configuration);

		if (_genesysConfig.Logging.UseLogging)
			GenesysHelper.UseGenesysLogging(configuration, _genesysConfig.Logging);

		var tokenInfo = await GetAccessTokenInfoAsync(configuration.ApiClient);

		if (tokenInfo.Suceeded)
			configuration.AccessToken = tokenInfo.Data.AccessToken;
		else
			throw new Exception($"code: {tokenInfo.Error?.Code}, Error: {tokenInfo.Error?.Message}");
	}

	public async Task<Result<AuthToken>> GetAccessTokenInfoAsync(ApiClient apiClient)
	{
		ArgumentNullException.ThrowIfNull(apiClient);

		try
		{
			var tokenInfo = await Task.Run(() =>
					apiClient.PostToken(
						_genesysConfig.ClientDetails.ClientId,
						_genesysConfig.ClientDetails.ClientSecret))
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