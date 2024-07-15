namespace Domain.Config.Genesys;

public class GenesysLoggerConfig
{
	public bool UseLogging { get; set; }
    public string? Level { get; set; }
	public string? Format { get; set; }
	public bool LogRequestBody { get; set; }
	public bool LogResponseBody { get; set; }
	public bool LogToConsole { get; set; }
	public string? LogFilePath { get; set; }
}
