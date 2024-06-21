using Shared.Wrapper;

namespace Application.Common.Interfaces.MQ;

public interface IMQWriterClient
{
	Task InitializeMQClientAsync(string queueManagerName, string queueName, IDictionary<string, object> mqProperties);
	Task<Result> ExecuteWithRetryAsync();
}