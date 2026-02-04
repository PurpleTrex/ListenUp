using Polly;
using Polly.Retry;

namespace ListenUp.App.Services;

public static class ResilienceService
{
    public static readonly ResiliencePipeline<HttpResponseMessage> HttpPipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
        .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(1),
            BackoffType = DelayBackoffType.Exponential,
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .HandleResult(response => !response.IsSuccessStatusCode)
                .Handle<HttpRequestException>()
                .Handle<TaskCanceledException>()
        })
        .AddTimeout(TimeSpan.FromSeconds(30))
        .Build();

    public static async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3)
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = maxRetries,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential
            })
            .Build();

        return await pipeline.ExecuteAsync(async ct => await operation(), CancellationToken.None);
    }
}
