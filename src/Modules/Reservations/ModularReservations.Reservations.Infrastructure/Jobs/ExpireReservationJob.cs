using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModularReservations.Reservations.Application.Commands.ExpireReservation;
using ModularReservations.Reservations.Domain.Interfaces.Repositories;
using ModularReservations.Shared.Exceptions;
using ModularReservations.Shared.Interfaces.Dispatcher;

namespace ModularReservations.Reservations.Infrastructure.Jobs;

public sealed class ExpireReservationJob : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ExpireReservationJob> _logger;

    public ExpireReservationJob(IServiceScopeFactory serviceScopeFactory, TimeProvider timeProvider,
        ILogger<ExpireReservationJob> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                var reservationRepository = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
                var now = _timeProvider.GetUtcNow().UtcDateTime;
                var reservations = await reservationRepository.GetExpiredPendingAsync(now, stoppingToken);
                foreach (var reservation in reservations)
                {
                    try
                    {
                        await using var innerScope = _serviceScopeFactory.CreateAsyncScope();
                        var dispatcher = innerScope.ServiceProvider.GetRequiredService<IDispatcher>();
                        var expireReservationCommand = new ExpireReservationCommand(reservation.Id);
                        await dispatcher.SendAsync(expireReservationCommand, stoppingToken);
                    }
                    catch (Exception ex) when (ex is AppException or DomainException)
                    {
                        _logger.LogWarning(ex, "Skipped expiring reservation {ReservationId}", reservation.Id);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        _logger.LogError(ex, "Error expiring reservation {ReservationId}", reservation.Id);
                    }
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogError(exception, "Expire reservations job failed. Retrying on next tick");
            }
        }
    }
}
