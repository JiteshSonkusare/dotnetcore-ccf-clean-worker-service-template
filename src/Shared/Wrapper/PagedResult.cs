namespace Shared.Wrapper;

public class PagedResult<TResult>
{
	public List<TResult> Data { get; set; } = new List<TResult>();
	public int TotalHits { get; set; }

	public PagedResult(List<TResult> data, int totalHits)
	{
		Data = data;
		TotalHits = totalHits;
	}
}
