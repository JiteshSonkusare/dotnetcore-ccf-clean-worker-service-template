using NLog;
using NLog.Web;
using WorkerService.Extensions;
using Application.Common.ExceptionHandlers;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
	logger.Debug("Service started!");

	var builder = Host.CreateDefaultBuilder(args)
		.ConfigureAppConfiguration((hostingContext, config) =>
		{
			hostingContext.SetEnvironmentConfiguration(config);
		})
		.ConfigureServices((hostContext, services) =>
		{
			services.LogDependencies(hostContext.Configuration)
			        .DatabaseDependencies(hostContext.Configuration)
					.ConfigOptionsDependencies(hostContext.Configuration)
					.AssemblyDependencies()
					.QuartzDependencies(hostContext.Configuration);
		})
		.UseWindowsService()
		.UseNLog()
		.Build();

	await builder.RunAsync();
}
catch (Exception ex)
{
	var error = ex.With(ex.Source, ex.Message, ex.StackTrace);
	logger.Error(error);
}
finally
{
	LogManager.Shutdown();
}
