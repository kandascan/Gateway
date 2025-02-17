using Gateway.ApplicationCore.Models;
using System.Collections.Concurrent;
using System.Threading.Channels;

public class ApiRequestQueue
{
    private readonly ILogger<ApiRequestQueue> _logger;
    private readonly Channel<IRequestTask> _queue = Channel.CreateUnbounded<IRequestTask>();
    private readonly ConcurrentDictionary<Guid, object> _cache = new();

    public ApiRequestQueue(ILogger<ApiRequestQueue> logger)
    {
        _logger = logger;
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

    public bool TrySetResult<T>(Guid requestId, T result)
    {
        return _cache.TryAdd(requestId, result!);
    }

    public bool TryGetResult<T>(Guid requestId, out T? result)
    {
        if (_cache.TryGetValue(requestId, out var value) && value is T typedValue)
        {
            result = typedValue;
            return true;
        }

        result = default;
        return false;
    }
}

// Interfejs bazowy dla generycznych RequestTask
public interface IRequestTask
{
    Guid RequestId { get; }
    Task Execute(ApiRequestQueue queue);
}

// Implementacja generycznego zadania
public class RequestTask<T> : IRequestTask
{
    public Guid RequestId { get; }
    private readonly Func<Task<T>> _request;

    public RequestTask(Guid requestId, Func<Task<T>> request)
    {
        RequestId = requestId;
        _request = request;
    }

    public async Task Execute(ApiRequestQueue queue)
    {
        try
        {
            var result = await _request();
            queue.TrySetResult(RequestId, result);
        }
        catch (Exception)
        {
            queue.TrySetResult(RequestId, default(T));
        }
    }
}
