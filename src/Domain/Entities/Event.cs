using Domain.Contracts;

namespace Domain.Entities;

public class Event : AuditableEntity<int>
{
	public string Note { get; set; } = null!;
	public string Status { get; set; } = null!;
	public DateTime? ProcessedOnUtc { get; set; }
}