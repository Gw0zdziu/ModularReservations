using ModularReservations.Reservations.Application.Interfaces;

namespace ModularReservations.Reservations.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ReservationsDbContext.ReservationsDbContext _dbContext;

    public UnitOfWork(ReservationsDbContext.ReservationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await  _dbContext.SaveChangesAsync(cancellationToken);
    }
}
