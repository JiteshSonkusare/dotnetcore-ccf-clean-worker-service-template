using Shared.Wrapper;

namespace Application.Common.Interfaces.MQ;

public interface IMQWriterClient
{
    Task<Result> ExecuteAsync(string requestMessage, CancellationToken cancellationToken, bool useRetry = false);
}