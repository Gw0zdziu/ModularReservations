using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularReservations.Shared.Interfaces.Dispatcher;

namespace ModularReservations.Shared.Infrastructure.Messaging.Outbox;

public class OutboxProcessor<TDbContext> : BackgroundService where TDbContext : DbContext
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OutboxProcessor(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var periodTimer = new PeriodicTimer(TimeSpan.FromSeconds(2));
        while (await periodTimer.WaitForNextTickAsync(stoppingToken))
        {
            var scope = _serviceScopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            var outboxMessages = await dbContext.Set<OutboxMessage>()
                .Where(x => x.ProcessedOn == null)
                .OrderBy(x => x.OccurredOn)
                .Take(35)
                .ToListAsync<OutboxMessage>(stoppingToken);
            foreach (var outboxMessage in outboxMessages)
            {
                var internalScope = _serviceScopeFactory.CreateAsyncScope();
                var dispatcher = internalScope.ServiceProvider.GetRequiredService<IDispatcher>();
            }
        }
    }
}
