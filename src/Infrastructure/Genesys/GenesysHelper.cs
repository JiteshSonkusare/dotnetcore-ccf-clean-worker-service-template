using PureCloudPlatform.Client.V2.Client;
using Application.Common.ExceptionHandlers;

namespace Infrastructure.Genesys;

public static class GenesysHelper
{
	public static PureCloudRegionHosts GetRegion(string configRegion)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(configRegion))
			{
				throw new ArgumentNullException(nameof(configRegion), "Genesys region must be provided.");
			}

			return Enum.Parse<PureCloudRegionHosts>(configRegion, true);
		}
		catch (Exception ex)
		{
			throw ex.With(ex.Message, ex.Source)
						.DetailData(nameof(configRegion), configRegion);
		}
	}
}