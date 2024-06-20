using IBM.WMQ;
using Polly.Registry;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace MQTestConsoleApp.Test3;

public interface IMQReaderClient : IDisposable
{
    Task InitializeMQClientAsync(string queueManagerName, string queueName, IDictionary<string, object> mqProperties);

    Task<MQMessage> ExecuteWithRetryAsync();
}

public class MQReaderClient(ResiliencePipelineProvider<string> pipelineProvider) : IMQReaderClient
{
    private MQQueue? _queue;
    private MQQueueManager? _queueManager;
    private readonly ResiliencePipelineProvider<string> _pipelineProvider = pipelineProvider;

    public async Task InitializeMQClientAsync(string queueManagerName, string queueName, IDictionary<string, object> mqProperties)
    {
        _queueManager = await CreateQueueManagerAsync(queueManagerName, mqProperties).ConfigureAwait(false);
        _queue = await AccessQueueAsync(queueName).ConfigureAwait(false);
    }

    public async Task<MQMessage> ExecuteWithRetryAsync()
    {
        var pipeline = _pipelineProvider.GetPipeline("default");

        try
        {
            var result = await pipeline.ExecuteAsync(
                async ct => await GetMessageAsync().ConfigureAwait(false))
                .ConfigureAwait(false);

            return result;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error executing with retry", ex);
        }
    }

    private async Task<MQMessage> GetMessageAsync()
    {
        var message = new MQMessage();
        try
        {
            await Task.Run(() => _queue!.Get(message)).ConfigureAwait(false);
            return message;
        }
        catch (MQException mqe)
        {
            if (mqe.ReasonCode == MQC.MQRC_NO_MSG_AVAILABLE)
                return message; // Return empty message or handle as needed

            throw new ApplicationException("MQ Read Error", mqe);
        }
    }

    private static Task<MQQueueManager> CreateQueueManagerAsync(string queueManagerName, IDictionary<string, object> mqProperties)
    {
        var connectionProperties = new Hashtable
            {
                { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED },
                { MQC.HOST_NAME_PROPERTY, mqProperties[MQC.HOST_NAME_PROPERTY] },
                { MQC.PORT_PROPERTY, mqProperties[MQC.PORT_PROPERTY] },
                { MQC.CHANNEL_PROPERTY, mqProperties[MQC.CHANNEL_PROPERTY] }
            };

        MQExtensions.AddOptionalProperties(connectionProperties, mqProperties);

        return Task.FromResult(new MQQueueManager(queueManagerName, connectionProperties));
    }

    private Task<MQQueue> AccessQueueAsync(string queueName)
    {
        return Task.Run(() => _queueManager!.AccessQueue(
            queueName,
            MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING));
    }

    public void Dispose()
    {
        if (_queue != null && _queue.IsOpen)
            _queue.Close();

        if (_queueManager != null && _queueManager.IsOpen)
            _queueManager.Close();
    }
}

public class Run
{
    static async Task Main(string[] args)
    {
        // Setup DI container
        var serviceProvider = ConfigureServices();

        // Resolve required service
        var client = serviceProvider.GetRequiredService<IMQReaderClient>();

        // Your MQ settings
        var mqSettings = new MQSettings
        {
            QueueManagerName = "MT94",
            QueueName = "GENESYS01.XML.FROM.PORTAL01",
            Properties = new Dictionary<string, object>
            {
                { MQC.HOST_NAME_PROPERTY, "mq-MT94.drf01.net" },
                { MQC.PORT_PROPERTY, 4350 },
                { MQC.CHANNEL_PROPERTY, "GENESYS01.MT94.CL01" }
            }
        };

        // Initialize and use MQ client
        await client.InitializeMQClientAsync(mqSettings.QueueManagerName, mqSettings.QueueName, mqSettings.Properties);
        var message = await client.ExecuteWithRetryAsync();
        Console.WriteLine($"Message received: {message.ReadString(message.MessageLength)}");
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Add services
        services.AddSingleton<IMQReaderClient, MQReaderClient>();

        // Add resilience pipeline
        services.ResilliencePipelineExtension();

        // Build the ServiceProvider
        return services.BuildServiceProvider();
    }
}