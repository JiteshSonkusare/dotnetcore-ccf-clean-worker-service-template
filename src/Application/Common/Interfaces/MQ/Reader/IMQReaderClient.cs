using IBM.WMQ;
using Shared.Wrapper;

namespace Application.Common.Interfaces.MQ;

public interface IMQReaderClient : IDisposable
{
    Task<Result<MQMessage>> ExecuteWithRetryAsync(CancellationToken cancellationToken);

    Task<Result<MQMessage>> ExecuteAsync(CancellationToken cancellationToken);
}