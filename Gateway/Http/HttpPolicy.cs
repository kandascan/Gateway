using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace Gateway.Http
{
    public static class HttpPolicy
    {
        public static IServiceCollection AddHttpPolicy(this IServiceCollection services)
        {
            services.ConfigureHttpClientDefaults(options =>
             options.AddResilienceHandler("my-pipeline", handler =>
             {
                 // Refer to https://www.pollydocs.org/strategies/retry.html#defaults for retry defaults
                 handler.AddRetry(new HttpRetryStrategyOptions
                 {
                     MaxRetryAttempts = 4,
                     Delay = TimeSpan.FromSeconds(2),
                     BackoffType = DelayBackoffType.Exponential
                 });

                 // Refer to https://www.pollydocs.org/strategies/timeout.html#defaults for timeout defaults
                 handler.AddTimeout(TimeSpan.FromSeconds(5));

                 handler.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                 {
                     FailureRatio = 0.5,                // Otworzy obwód, jeśli 50% requestów w czasie SamplingDuration nie powiedzie się
                     SamplingDuration = TimeSpan.FromSeconds(10), // Okres próbkowania błędów
                     MinimumThroughput = 2,             // Co najmniej 2 requesty muszą być analizowane
                     BreakDuration = TimeSpan.FromSeconds(30) // Czas "otwartego obwodu" - system nie wysyła requestów przez 30 sekund
                 });
             }));

            return services;
        }
    }
}
