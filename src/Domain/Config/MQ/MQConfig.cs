namespace Domain.Config.MQ;

public record MQConfig
{
	public const string SectionName = "MQ";

    public MQReaderConfig? MQReader { get; set; }

    public MQWriterConfig? MQWriter { get; set; }
}