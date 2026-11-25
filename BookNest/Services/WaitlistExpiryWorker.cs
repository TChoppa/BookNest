using BookNest.IServices;
using Microsoft.Extensions.Options;

namespace BookNest.Services
{
    public class WaitlistExpiryWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WaitlistExpiryWorker> _logger;
        private readonly TimeSpan _interval;

        public WaitlistExpiryWorker(
            IServiceScopeFactory scopeFactory,
            IOptions<WaitlistOptions> options,
            ILogger<WaitlistExpiryWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _interval = TimeSpan.FromSeconds(options.Value.ExpiryCheckIntervalSeconds);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_interval);
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var waitlistService = scope.ServiceProvider.GetRequiredService<IBookWaitlistService>();
                    await waitlistService.ProcessExpiredOffersAsync();
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Failed to process expired book waitlist offers.");
                }
            }
        }
    }
}
