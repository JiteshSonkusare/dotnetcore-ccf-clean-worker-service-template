using System.ComponentModel.DataAnnotations;

namespace Domain.Config.MQ;

public record MQSettings
{
	public const string SectionName = "MQ";

	[Required(ErrorMessage = "Queue manager name required!")]
	public string? QueueManagerName { get; set; }

	[Required(ErrorMessage = "Queue name required!")]
	public string? QueueName { get; set; }

	public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}