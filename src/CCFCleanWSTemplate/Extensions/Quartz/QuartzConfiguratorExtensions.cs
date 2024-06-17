using Quartz;
using System.Reflection;
using Application.Common.ExceptionHandlers;

namespace WorkerService.Extensions.Quartz;

public static class QuartzConfiguratorExtensions
{
	public static void AddJobAndTrigger<T>(
		this IServiceCollectionQuartzConfigurator quartz,
		IConfiguration configuration)
		where T : IJob
	{
		try
		{
			string jobName = typeof(T).Name;
			var configKey = $"Quartz:{jobName}";
			var cronSchedule = configuration[configKey];

			if (string.IsNullOrEmpty(cronSchedule))
			{
				throw new ArgumentNullException($"No Quartz.NET Cron schedule found for job in configuration at {configKey}");
			}

			var jobKey = new JobKey(jobName);
			quartz.
				AddJob<T>(jobs => jobs.WithIdentity(jobKey).StoreDurably())
					.AddTrigger(trigger => trigger
						.ForJob(jobKey)
							.WithIdentity(jobName + "-trigger")
								.WithCronSchedule(cronSchedule));
		}
		catch (Exception ex)
		{
			ex.With(ex.Source, ex.Message, ex.InnerException?.Message, ex.StackTrace);
		}
	}

	public static void AddJobsAndTriggers(
		this IServiceCollectionQuartzConfigurator quartz,
		IConfiguration configuration)
	{
		var jobType = typeof(IJob);
		var jobTypes = Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where(t => jobType.IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);

		if (!jobTypes.Any())
			return;

		foreach (var type in jobTypes)
		{
			string jobName = type.Name;
			var configKey = $"Quartz:{jobName}";
			var cronSchedule = configuration[configKey];

			if (string.IsNullOrEmpty(cronSchedule))
			{
				throw new ArgumentNullException($"No Quartz.NET Cron schedule found for job in configuration at {configKey}");
			}

			var jobKey = new JobKey(jobName);
			quartz.AddJob(type, jobKey, jobs => jobs.WithIdentity(jobKey).StoreDurably())
					.AddTrigger(trigger => trigger
					  .ForJob(jobKey)
						.WithIdentity($"{jobName}-trigger")
							.WithCronSchedule(cronSchedule));
		}
	}
}