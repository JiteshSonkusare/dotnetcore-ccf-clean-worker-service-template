using IBM.WMQ;
using Shared.Wrapper;
using Polly.Registry;
using Domain.Config.MQ;
using System.Collections;
using Microsoft.Extensions.Options;
using static Domain.Errors.ErrorDto;
using Application.Common.Interfaces.MQ;
using Application.Common.ExceptionHandlers;

namespace Infrastructure.MQ;

public class MQReaderClient(
    IOptions<MQReaderConfig> config,
    ResiliencePipelineProvider<string> pipelineProvider)
    : IMQReaderClient
{
    private MQQueue? _queue;
    private MQQueueManager? _queueManager;
    private readonly MQReaderConfig _config = config.Value;
    private readonly ResiliencePipelineProvider<string> _pipelineProvider = pipelineProvider;

    public async Task<Result<MQMessage>> ExecuteAsync(CancellationToken cancellationToken, bool useRetry = false)
    {
        await InitializeMQClientAsync();
        if (useRetry)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            return  await pipeline.ExecuteAsync(
                        async ct => await GetMessageAsync(cancellationToken)
                            .ConfigureAwait(false), cancellationToken)
                        .ConfigureAwait(false);

        }
        else
        {
            return await GetMessageAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    public async Task InitializeMQClientAsync()
    {
        _queueManager = await CreateQueueManagerAsync(
                _config.QueueManagerName,
                _config.Properties)
            .ConfigureAwait(false);

        _queue = await AccessQueueAsync(
                _config.QueueName)
            .ConfigureAwait(false);
    }

    private async Task<Result<MQMessage>> GetMessageAsync(CancellationToken cancellationToken)
    {
        var message = new MQMessage();
        try
        {
            await Task.Run(
                () => _queue!.Get(message), cancellationToken)
                .ConfigureAwait(false);

            return Result.Success(message);
        }
        catch (MQException mqe)
        {
            if (mqe.ReasonCode == MQC.MQRC_NO_MSG_AVAILABLE)
                return Result.Failure<MQMessage>(MQErrors.MQMessageError(mqe.ReasonCode, mqe.Message));

            throw mqe.With("MQ Read Error", mqe.Source, mqe.Message);
        }
    }

    private static Task<MQQueueManager> CreateQueueManagerAsync(string queueManagerName, IDictionary<string, object> mqProperties)
    {
        try
        {
            var connectionProperties = new Hashtable
        {
            { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED },
            { MQC.HOST_NAME_PROPERTY, mqProperties[MQC.HOST_NAME_PROPERTY] },
            { MQC.PORT_PROPERTY, mqProperties[MQC.PORT_PROPERTY] },
            { MQC.CHANNEL_PROPERTY, mqProperties[MQC.CHANNEL_PROPERTY] }
        };

            MQHelper.AddOptionalProperties(connectionProperties, mqProperties);

            return Task.FromResult(new MQQueueManager(queueManagerName, connectionProperties));
        }
        catch (Exception ex)
        {
            throw ex.With(ex.Source, ex.Message);
        }
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