using IBM.WMQ;
using Shared.Wrapper;

namespace Application.Common.Interfaces.MQ;

public interface IMQReaderClient : IDisposable
{
    Task<Result<MQMessage>> ExecuteAsync(CancellationToken cancellationToken, bool UseRetry = false);
}