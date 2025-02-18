
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
            await queue.TrySetResultAsync(RequestId, result);
        }
        catch (Exception)
        {
            await queue.TrySetResultAsync(RequestId, default(T));
        }
    }
}

