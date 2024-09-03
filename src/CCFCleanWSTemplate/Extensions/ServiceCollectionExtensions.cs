using Quartz;
using NLog.Web;
using Shared.Extension;
using Domain.Config.MQ;
using Domain.Config.Genesys;
using Infrastructure.Context;
using Application.Extensions;
using NLog.Extensions.Logging;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using WorkerService.Extensions.Quartz;

namespace WorkerService.Extensions;

public static class ServiceCollectionExtensions
{
	#region Log

	internal static IServiceCollection LogDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddLogging(logging =>
		{
			logging.ClearProviders()
				   .SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace)
				   .AddNLog(configuration);
		});
		return services;
	}

	#endregion

	#region Database

	internal static IServiceCollection DatabaseDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		return services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlConnection")));
	}

	#endregion

	#region Qaurtz

	internal static IServiceCollection QuartzDependencies(this IServiceCollection services, IConfiguration configuration)
	{ 
		services.AddQuartz(q =>
		{
			q.AddJobListener<JobExceptionListener>();
			q.AddJobsAndTriggers(configuration);
		});
		services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

		return services;
	}

	#endregion

	#region Configuration

	public static IServiceCollection ConfigOptionsDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddOptions<GenesysConfig>()
				.Bind(configuration.GetSection(GenesysConfig.SectionName))
				.ValidateDataAnnotations();

		services.AddOptions<MQConfig>()
				.Bind(configuration.GetSection(MQConfig.SectionName))
				.ValidateDataAnnotations();

		return services;
	}

	#endregion

	#region Other Project Dependency

	public static IServiceCollection AssemblyDependencies(this IServiceCollection services)
	{
		services.InfrastructureDependencies()
				.ApplicationDependencies()
				.SharedDependencies();

		return services;
	}

	#endregion

	#region Environment Configuration

	public static void SetEnvironmentConfiguration(this HostBuilderContext hostingContext, IConfigurationBuilder configuration)
	{
		var env = hostingContext.HostingEnvironment;
		var root = hostingContext.HostingEnvironment.ContentRootPath;
		
		configuration.SetBasePath(root);
		configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
		configuration.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
		configuration.AddEnvironmentVariables();
	}

	#endregion
}
