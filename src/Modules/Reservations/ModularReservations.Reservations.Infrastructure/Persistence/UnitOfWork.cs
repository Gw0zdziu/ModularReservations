using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ModularReservations.Reservations.Application.Interfaces;
using ModularReservations.Reservations.Infrastructure.DbContextReservations;
using ModularReservations.Reservations.Infrastructure.Exceptions;
using ModularReservations.Shared.Aggregates;
using ModularReservations.Shared.Infrastructure.Messaging;
using ModularReservations.Shared.Infrastructure.Messaging.Outbox;

namespace ModularReservations.Reservations.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ReservationsDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly IOptions<EventTypeOptions> _eventTypeOptions;

    public UnitOfWork(ReservationsDbContext dbContext, TimeProvider timeProvider, IOptions<EventTypeOptions> eventTypeOptions)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _eventTypeOptions = eventTypeOptions;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        try
        {
            var entityEntries = _dbContext.ChangeTracker.Entries<AggregateRoot>().ToList();
            var events = entityEntries.SelectMany(e => e.Entity.Events);
            var outboxMessages = events.Select(x =>
            {
                var now = _timeProvider.GetUtcNow().UtcDateTime;
                var  isGetValue = _eventTypeOptions.Value.TryGetKey(x.GetType(), out var value);
                if (!isGetValue)
                {
                    throw new Exception($"Event type not found for {x.GetType().FullName}");
                }
                return new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Content = JsonSerializer.Serialize(x, x.GetType()),
                    Type = value,
                    OccurredOn = now,
                };
            }).ToList();
            foreach (var entityEntry in entityEntries)
            {
                entityEntry.Entity.ClearEvents();
            }
            _dbContext.AddRange(outboxMessages);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            var entityEntry = exception.Entries.FirstOrDefault();
            if (entityEntry is null)
            {
                throw;
            }

            var entityName = entityEntry.Metadata.ClrType.Name;
            var property = entityEntry.Metadata.FindPrimaryKey()?.Properties[0];
            if (property is null)
            {
                throw;
            }
            var originalValue = entityEntry.OriginalValues[property.Name];
            var entityId = $"{property.Name}={originalValue}";
            throw new ConcurrencyConflictException(entityName, entityId, exception);
        }
    }
}
