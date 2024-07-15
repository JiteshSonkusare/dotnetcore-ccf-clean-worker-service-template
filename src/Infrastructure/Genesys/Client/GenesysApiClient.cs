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
		return await ExecuteAsync(action, useRetry);
	}

	public TResult Execute<TResult>(Func<TResult> action, bool useRetry = false)
	{
		return Execute(action, useRetry);
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
}