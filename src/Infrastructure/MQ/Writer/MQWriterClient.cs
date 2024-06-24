using IBM.WMQ;
using Polly.Registry;
using Shared.Wrapper;
using Domain.Config.MQ;
using System.Collections;
using Microsoft.Extensions.Options;
using Application.Common.Interfaces.MQ;
using Application.Common.ExceptionHandlers;

namespace Infrastructure.MQ;

public class MQWriterClient(
    IOptions<MQWriterConfig> config,
    ResiliencePipelineProvider<string> pipelineProvider)
    : IMQWriterClient
{
    private MQQueueManager? _queueManager;
    private readonly MQWriterConfig _config = config.Value;
    private readonly ResiliencePipelineProvider<string> _pipelineProvider = pipelineProvider;

    public async Task<Result> ExecuteAsync(string requestMessage, CancellationToken cancellationToken, bool useRetry = false)
    {
        await InitializeMQClientAsync();
        if (useRetry)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");

            return await pipeline.ExecuteAsync(
                async ct => await SendMessageAsync(
                    _config.QueueName,
                    requestMessage)
                .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            return await SendMessageAsync(
                    _config.QueueName,
                    requestMessage)
                .ConfigureAwait(false);
        }
    }

    public async Task InitializeMQClientAsync()
    {
        _queueManager = await CreateQueueManagerAsync().ConfigureAwait(false);
    }

    private Task<MQQueueManager> CreateQueueManagerAsync()
    {
        try
        {
            var connectionProperties = new Hashtable
            {
                { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED },
                { MQC.HOST_NAME_PROPERTY, _config.Properties[MQC.HOST_NAME_PROPERTY] },
                //{ MQC.PORT_PROPERTY, _config.Properties[MQC.PORT_PROPERTY] },
                { MQC.CHANNEL_PROPERTY, _config.Properties[MQC.CHANNEL_PROPERTY] }
            };
            return Task.FromResult(new MQQueueManager(
                    _config.QueueManagerName,
                    connectionProperties));
        }
        catch (Exception ex)
        {
            throw ex.With(ex.Source, ex.Message);
        }
    }

    private async Task<Result> SendMessageAsync(string queue, string message)
    {
        try
        {
            var QueueMsg = new MQMessage
            {
                Expiry = MQC.MQEI_UNLIMITED
            };
            QueueMsg.WriteString(message);
            QueueMsg.Format = MQC.MQFMT_STRING;

            await Task.Run(() => _queueManager!.Put(
                queue,
                QueueMsg))
                .ConfigureAwait(false);

            return Result.Success();
        }
        catch (MQException mqe)
        {
            throw mqe.With("MQ write error", mqe.Source, mqe.Message);
        }
    }

    public void Dispose()
    {
        if (_queueManager != null && _queueManager.IsOpen)
            _queueManager.Close();
    }
}
