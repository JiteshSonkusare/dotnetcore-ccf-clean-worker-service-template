using Shared.Wrapper;

namespace Application.Common.Interfaces;

public interface IGenesysApiClient
{
    T CreateApiInstance<T>() where T : class;

    Task<TResult> ExecuteWithRetryAsync<TResult>(Func<Task<TResult>> action);

    Task<Result<T>> ExecuteWithRetryAsync<T>(Func<Task<Result<T>>> action);

    TResult ExecuteWithRetry<TResult>(Func<TResult> action);

    Result<T> ExecuteWithRetry<T>(Func<Result<T>> action);
}