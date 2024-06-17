using NLog;
using MediatR;
using Shared.Wrapper;
using System.Text.Json;
using Application.Common.Interfaces;

namespace Application.Common.Behaviors;

public class RequestResponseLoggingBehavior<TRequest, TResponse>(IDateTimeService dateTimeService)
	: IPipelineBehavior<TRequest, TResponse>
	  where TRequest : IRequest<TResponse>
	  where TResponse : Result
{
	private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
	private readonly IDateTimeService _dateTimeService = dateTimeService;

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var correlationId = Guid.NewGuid();

		// Log request
		_logger.Info($"Handling request {correlationId}: {typeof(TRequest).Name}");

		var result = await next();

		// Log error
		if (result.IsFailure)
			_logger.Error($"Request failure " +
				$"{correlationId}, " +
				$"{typeof(TRequest).Name}, " +
				$"{result.Error}, " +
				$"{_dateTimeService.Now}");
		else
		{
			// Log response
			var responseJson = JsonSerializer.Serialize(result);
			_logger.Info($"Response for {correlationId}: {responseJson}");
		}

		return result;
	}
}