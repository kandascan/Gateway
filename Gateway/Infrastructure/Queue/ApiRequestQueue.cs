using Gateway.Cache;
using System.Threading.Channels;

public class ApiRequestQueue
{
    private readonly ILogger<ApiRequestQueue> _logger;
    private readonly Channel<IRequestTask> _queue = Channel.CreateUnbounded<IRequestTask>();
    private readonly RedisCacheService _cacheService;

    public ApiRequestQueue(ILogger<ApiRequestQueue> logger, RedisCacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public int Count => _queue.Reader.Count;

    public Guid Enqueue<T>(Func<Task<T>> request)
    {
        var requestId = Guid.NewGuid();
        var task = new RequestTask<T>(requestId, request);
        _queue.Writer.WriteAsync(task);

        _logger.LogInformation($"Dodano zadanie do kolejki. RequestId: {requestId}. Liczba zadań w kolejce: {Count}");

        return requestId;
    }

    public async Task<IRequestTask> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }

    public async Task<bool> TrySetResultAsync<T>(Guid requestId, T result)
    {        
        return await _cacheService.SetValueAsync(requestId, result);
    }

    public async Task<(bool found, T? result)> TryGetResultAsync<T>(Guid requestId)
    {
        var result = await _cacheService.GetValueAsync<T>(requestId);
        return (result is not null, result);
    }
}