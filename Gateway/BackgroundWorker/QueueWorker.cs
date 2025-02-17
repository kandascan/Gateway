namespace Gateway.BackgroundWorker
{
    public class QueueWorker : BackgroundService
    {
        private readonly ApiRequestQueue _queue;
        private readonly ILogger<QueueWorker> _logger;

        public QueueWorker(ApiRequestQueue queue, ILogger<QueueWorker> logger)
        {
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var requestTask = await _queue.DequeueAsync(stoppingToken);
                _logger.LogInformation($"Ilosc zadań w kolejce: {_queue.Count}");

                await requestTask.Execute(_queue);

                _logger.LogInformation($"Zadanie zakończone. RequestId: {requestTask.RequestId}");

                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}
