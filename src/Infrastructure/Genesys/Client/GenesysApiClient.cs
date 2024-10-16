using Shared.Wrapper;
using Application.Common.Interfaces;
using PureCloudPlatform.Client.V2.Client;
using Application.Common.Interfaces.Genesys;

namespace Infrastructure.Genesys.Client;

public class GenesysApiClient(
	IGenesysConfigurationHandler genesysConfigurationHandler)
	: IGenesysApiClient
{
	private Configuration Configuration { get; set; } = new();
	private readonly IGenesysConfigurationHandler _genesysConfigurationHandler = genesysConfigurationHandler;

	public T CreateApiInstance<T>() where T : class
	{
		var constructor = typeof(T).GetConstructor(new[] { typeof(Configuration) });

		if (constructor == null)
			throw new InvalidOperationException($"Type {typeof(T).Name} does not have a constructor with a Configuration parameter.");

		return (T)constructor.Invoke(new object[] { Configuration });
	}

	public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action, bool useRetry = false)
	{
		await _genesysConfigurationHandler.InitializeConfigurationAsync(Configuration, useRetry);
		return await action();
	}

	public TResult Execute<TResult>(Func<TResult> action, bool useRetry = false)
	{
		_genesysConfigurationHandler.InitializeConfigurationAsync(Configuration, useRetry).GetAwaiter().GetResult();
		return action();
	}

	public async Task ExecuteAsync(Func<Task> action, bool useRetry = false)
	{
		await _genesysConfigurationHandler.InitializeConfigurationAsync(Configuration, useRetry);
		await action();
	}

	public void Execute(Action action, bool useRetry = false)
	{
		_genesysConfigurationHandler.InitializeConfigurationAsync(Configuration, useRetry).GetAwaiter().GetResult();
		action();
	}

	public async Task<PagedResult<TResult>> ExecuteWithPagingAsync<TRequest, TResponse, TResult>(
		Func<int, TRequest> createRequest,
		Func<TRequest, Task<TResponse>> action,
		Func<TResponse, IEnumerable<TResult>> extractResults,
		Func<TResponse, int?>? extractTotalHits = null,
		bool useRetry = false,
		CancellationToken cancellationToken = default)
	{
		await _genesysConfigurationHandler.InitializeConfigurationAsync(Configuration, useRetry);

		var allResults = new List<TResult>();
		int currentPage = 1;
		int totalHits = 0;

		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var request = createRequest(currentPage);
			var response = await ExecuteAsync(() => action(request), useRetry);

			var pageResults = extractResults(response) ?? Enumerable.Empty<TResult>();

			if (!pageResults.Any()) break;

			allResults.AddRange(pageResults);

			if (currentPage == 1 && extractTotalHits != null)
			{
				totalHits = extractTotalHits(response) ?? 0;

				if (totalHits == 0) break;
			}

			if (totalHits > 0 && allResults.Count >= totalHits) break;

			currentPage++;
		}

		return new PagedResult<TResult>(allResults, allResults.Count);
	}
}