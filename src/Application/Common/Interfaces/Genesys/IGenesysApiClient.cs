namespace Application.Common.Interfaces;

public interface IGenesysApiClient
{
    T CreateApiInstance<T>() where T : class;

    Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action, bool useRetry = false);

    TResult Execute<TResult>(Func<TResult> action, bool useRetry = false);
}