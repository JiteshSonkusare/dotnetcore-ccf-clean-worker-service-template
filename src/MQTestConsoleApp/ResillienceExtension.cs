using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;

namespace MQTestConsoleApp;

public static class ResillienceExtension
{
    public static IServiceCollection ResilliencePipelineExtension(this IServiceCollection services)
    {
        services.AddResiliencePipeline("default", x =>
        {
            x.AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(5),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true
            })
            .AddTimeout(TimeSpan.FromSeconds(30));
        });

        return services;
    }
}
