using IBM.WMQ;
using Shared.Wrapper;

namespace Application.Common.Interfaces.MQ;

public interface IMQReaderClient : IDisposable
{
    Task InitializeMQClientAsync(string queueManagerName, string queueName, IDictionary<string, object> mqProperties);
    Task<Result<MQMessage>> ExecuteWithRetryAsync();
}