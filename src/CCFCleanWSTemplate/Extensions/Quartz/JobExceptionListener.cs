using Quartz;
using Shared.Extension;

namespace WorkerService.Extensions.Quartz;

public class JobExceptionListener(ILogger<JobExceptionListener> logger) : IJobListener
{
	public string Name => "QuartzJobExceptionListener";
	private readonly ILogger<JobExceptionListener> _logger = logger;

	public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
	{
		return Task.CompletedTask;
	}

	public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
	{
		return Task.CompletedTask;
	}

	public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
	{
		if (jobException != null)
		{
			var (rootException, exceptionData) = GetRootInnerException(jobException);
			var jobDetails = new
			{
				JobName          = context.JobDetail.Key.Name,
				Source           = rootException.Source ?? string.Empty,
				Message          = rootException.Message ?? string.Empty,
				Error            = jobException.Message ?? string.Empty,
				ErrorDescription = rootException.StackTrace ?? string.Empty,
				Data             = exceptionData
			};

			var exceptionJson = jobDetails.ToJson();
			_logger.LogError(exceptionJson);
		}

		return Task.CompletedTask;
	}

	private static (Exception RootException, Dictionary<string, object?> Extensions) GetRootInnerException(Exception exception)
	{
		if (exception == null)
		{
			throw new ArgumentNullException(nameof(exception), "Exception cannot be null.");
		}

		var extensions = new Dictionary<string, object?>(StringComparer.Ordinal);

		// Traverse through the inner exceptions to reach the root exception
		Exception currentException = exception;
		while (currentException.InnerException != null)
		{
			currentException = currentException.InnerException;
		}

		// Collect data from each exception in the chain
		Exception? tempException = exception;
		while (tempException != null)
		{
			if (tempException.Data != null)
			{
				foreach (var key in tempException.Data.Keys)
				{
					if (key is not null && tempException.Data[key] is not null)
					{
						extensions[key.ToString() ?? $"{key.ToString()}-key"] = tempException.Data[key];
					}
				}
			}
			tempException = tempException.InnerException;
		}

		return (currentException, extensions);
	}


}
