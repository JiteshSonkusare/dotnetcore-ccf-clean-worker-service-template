namespace Domain.Models.Genesys;

public class EventRequest
{
	public string Key { get; set; } = null!;
	public string? Note { get; set; }
	public string? Status { get; set; }
}