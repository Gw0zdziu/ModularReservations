using Microsoft.EntityFrameworkCore;
using ModularReservations.Reservations.Application.Interfaces;
using ModularReservations.Reservations.Infrastructure.DbContextReservations;
using ModularReservations.Reservations.Infrastructure.Exceptions;

namespace ModularReservations.Reservations.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ReservationsDbContext _dbContext;

    public UnitOfWork(ReservationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        try
        {
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
