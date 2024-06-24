using Shared.Wrapper;

namespace Application.Common.Interfaces;

public interface IGenesysApiClient
{
    T CreateApiInstance<T>() where T : class;

    Task<Result<T>> ExecuteWithRetryAsync<T>(Func<Task<Result<T>>> action);
    Result<T> ExecuteWithRetry<T>(Func<Result<T>> action);

    Task<TResult> ExecuteWithRetryAsync<TResult>(Func<Task<TResult>> action);
    TResult ExecuteWithRetry<TResult>(Func<TResult> action);

    Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action);
    TResult Execute<TResult>(Func<TResult> action);
}