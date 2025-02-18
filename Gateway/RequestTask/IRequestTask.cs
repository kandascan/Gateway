
// Interfejs bazowy dla generycznych RequestTask
public interface IRequestTask
{
    Guid RequestId { get; }
    Task Execute(ApiRequestQueue queue);
}

