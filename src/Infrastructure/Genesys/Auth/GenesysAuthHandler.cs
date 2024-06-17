using Shared.Wrapper;
using Domain.Models.Genesys;
using static Domain.Errors.JobErrors;
using PureCloudPlatform.Client.V2.Client;
using Application.Common.ExceptionHandlers;
using Application.Common.Interfaces.Genesys;
using PureCloudPlatform.Client.V2.Extensions;

namespace Infrastructure.Genesys.Auth;

public class GenesysAuthHandler : IGenesysAuthHandler
{
	public async Task<Result<AuthToken>> GetAccessTokenInfoAsync(string? clientId, string? clientSecret, ApiClient apiClient)
	{
		try
		{
			var tokenInfo = await Task.Run(
				() => apiClient.PostToken(clientId, clientSecret)).ConfigureAwait(true);

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
			var exception = ex.With(ex.Source, ex.Message, ex.StackTrace);
			throw exception;
		}
	}
}