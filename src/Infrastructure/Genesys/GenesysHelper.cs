using Domain.Config.Genesys;
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

	public static void UseRetry(Configuration configuration)
	{
		var retryConfig = new ApiClient.RetryConfiguration
		{
			RetryMax = 3,
			MaxRetryTimeSec = 10
		};

		configuration.ApiClient.RetryConfig = retryConfig;
	}

	public static void UseGenesysLogging(Configuration configuration, GenesysLoggerConfig genesysLoggerConfig)
	{
		configuration.Logger.Level = Enum.TryParse<LogLevel>(genesysLoggerConfig.Level, true, out var logLevel) 
			? logLevel 
			: LogLevel.LTrace;

		configuration.Logger.Format = Enum.TryParse<LogFormat>(genesysLoggerConfig.Format, true, out var logFormat)
			? logFormat 
			: LogFormat.JSON;

		configuration.Logger.LogRequestBody = genesysLoggerConfig.LogRequestBody;
		configuration.Logger.LogResponseBody = genesysLoggerConfig.LogResponseBody;
		configuration.Logger.LogToConsole = genesysLoggerConfig.LogToConsole;
		configuration.Logger.LogFilePath = genesysLoggerConfig.LogFilePath;
	}
}