using Shared.Wrapper;
namespace Application.Common.Interfaces;

public interface IGenesysApiClient
{
    T CreateApiInstance<T>() where T : class;

    Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action, bool useRetry = false);

    TResult Execute<TResult>(Func<TResult> action, bool useRetry = false);

	Task ExecuteAsync(Func<Task> action, bool useRetry = false);

	void Execute(Action action, bool useRetry = false);

	Task<PagedResult<TResult>> ExecuteWithPagingAsync<TRequest, TResponse, TResult>(
		Func<int, TRequest> createRequest,
		Func<TRequest, Task<TResponse>> action,
		Func<TResponse, IEnumerable<TResult>> extractResults,
		Func<TResponse, int?>? extractTotalHits = null,
		bool useRetry = false,
		CancellationToken cancellationToken = default);
}