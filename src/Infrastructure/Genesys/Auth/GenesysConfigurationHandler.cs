using Shared.Wrapper;
using Domain.Models.Genesys;
using Domain.Config.Genesys;
using Microsoft.Extensions.Options;
using static Domain.Errors.ErrorDto;
using Application.Common.Interfaces;
using PureCloudPlatform.Client.V2.Client;
using Application.Common.ExceptionHandlers;
using Application.Common.Interfaces.Genesys;
using PureCloudPlatform.Client.V2.Extensions;

namespace Infrastructure.Genesys.Auth;

public class GenesysConfigurationHandler(
	IOptions<GenesysConfig> genesysConfig,
	IDateTimeService dateTimeService)
	: IGenesysConfigurationHandler
{
	private readonly GenesysConfig _genesysConfig = genesysConfig.Value;
	private AuthToken? _cachedToken;
	private DateTime _tokenExpiration;
	private readonly IDateTimeService _dateTimeService = dateTimeService;
	private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

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
			configuration.AccessToken = tokenInfo.Data?.AccessToken;
		else
			throw new Exception($"code: {tokenInfo.Error?.Code}, Error: {tokenInfo.Error?.Message}");
	}

	public async Task<Result<AuthToken>> GetAccessTokenInfoAsync(ApiClient apiClient)
	{
		ArgumentNullException.ThrowIfNull(apiClient);

		await _semaphore.WaitAsync();

		try
		{
			if (_cachedToken != null && _tokenExpiration > _dateTimeService.Now)
			{
				return Result.Success(_cachedToken);
			}

			return await RequestNewAccessTokenAsync(apiClient);
		}
		finally
		{
			_semaphore.Release();
		}
	}

	private async Task<Result<AuthToken>> RequestNewAccessTokenAsync(ApiClient apiClient)
	{
		try
		{
			var tokenInfo = await Task.Run(() =>
					apiClient.PostToken(
						_genesysConfig.ClientDetails.ClientId,
						_genesysConfig.ClientDetails.ClientSecret))
					.ConfigureAwait(false);

			if (!string.IsNullOrWhiteSpace(tokenInfo.AccessToken))
			{
				_tokenExpiration = tokenInfo.ExpiresIn.HasValue
					? DateTime.UtcNow.AddSeconds(tokenInfo.ExpiresIn.Value - 300) // Subtract 5 mins buffer if ExpiresIn is provided
					: DateTime.UtcNow.AddMinutes(55); // Default to 55 minutes if no ExpiresIn provided

				_cachedToken = new AuthToken(
					tokenInfo.AccessToken,
					tokenInfo.RefreshToken,
					tokenInfo.TokenType,
					tokenInfo.ExpiresIn,
					tokenInfo.Error);

				return Result.Success(_cachedToken);
			}

			return Result.Failure<AuthToken>(
				AuthErrors.AuthTokenError(tokenInfo.Error?.ToString() ?? "Unknown token error."));
		}
		catch (Exception ex)
		{
			throw ex.With(ex.Source, ex.Message);
		}
	}
}