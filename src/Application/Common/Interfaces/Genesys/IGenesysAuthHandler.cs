using Shared.Wrapper;
using Domain.Models.Genesys;

using PureCloudPlatform.Client.V2.Client;

namespace Application.Common.Interfaces.Genesys;

public interface IGenesysAuthHandler
{
	Task<Result<AuthToken>> GetAccessTokenInfoAsync(string? clientId, string? clientSecret, ApiClient apiClient);
}