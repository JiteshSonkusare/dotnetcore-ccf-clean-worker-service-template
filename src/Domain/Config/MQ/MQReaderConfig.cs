using System.ComponentModel.DataAnnotations;

namespace Domain.Config.MQ;

public record MQReaderConfig
{
    [Required(ErrorMessage = "Queue manager name required!")]
    public string QueueManagerName { get; set; } = null!;

    [Required(ErrorMessage = "Queue name required!")]
    public string QueueName { get; set; } = null!;

    public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}
