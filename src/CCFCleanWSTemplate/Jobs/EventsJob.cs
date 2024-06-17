using Quartz;
using MediatR;
using Application.Common.ExceptionHandlers;
using Application.Tasks.SendEvent.Commands;

namespace WorkerService.Jobs;

public class EventsJob(ISender sender, ILogger<EventsJob> logger) : IJob
{
	private readonly ISender Sender = sender;
	private readonly ILogger<EventsJob> _logger = logger;

	public async Task Execute(IJobExecutionContext context)
	{
		try
		{
			_logger.LogInformation($"{nameof(EventsJob)} started!");

			var result = await Sender.Send(new SendEventCommand());
		}
		catch (Exception ex)
		{
			throw ex.With();
		}
	}
}