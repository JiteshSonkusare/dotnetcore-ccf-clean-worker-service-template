namespace Domain.Config.MQ;

public record MQConfig
{
	public const string SectionName = "MQ";

    public MQReaderConfig? MQReaderConfig { get; set; }

    public MQWriterConfig? MQWriterConfig { get; set; }
}