using Domain.Models.Genesys;
using PureCloudPlatform.Client.V2.Client;
using Shared.Wrapper;

namespace Application.Common.Interfaces.Genesys;

public interface IGenesysConfigurationHandler
{
	Task InitializeConfigurationAsync(Configuration configuration, bool useRetry = false);

	Task<Result<AuthToken>> GetAccessTokenInfoAsync(ApiClient apiClient);
}