﻿using Application.Common.Interfaces.MQ;
using IBM.WMQ;
using PureCloudPlatform.Client.V2.Model;
using Shared.Wrapper;

namespace Infrastructure.MQ;

public class MQWriterClient : IMQWriterClient
{
<<<<<<< HEAD

=======
	private readonly 

    public MQWriterClient()
    {
            
    }

	public Task InitializeMQClientAsync(string queueManagerName, string queueName, IDictionary<string, object> mqProperties)
	{
		MQQueueManager mQueueManager = new MQQueueManager(_smsConfig.Manager, mConnectionProperties);
	}

	public Task<Result> ExecuteWithRetryAsync()
	{
		throw new NotImplementedException();
	}
>>>>>>> 05a02117190498c5eff9eb2f4f171f421b10141c
}
