using Microsoft.EntityFrameworkCore;
using ModularReservations.Reservations.Domain.Aggregates;
using ModularReservations.Reservations.Domain.Interfaces.Repositories;
using ModularReservations.Reservations.Domain.Statuses;

namespace ModularReservations.Reservations.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly ReservationsDbContext.ReservationsDbContext _dbContext;

    public ReservationRepository(ReservationsDbContext.ReservationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Reservation?> GetByIdAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reservations
            .Where(x => x.Id == reservationId)
            .FirstOrDefaultAsync(cancellationToken);

    }

    public void Update(Reservation reservation)
    {
        _dbContext.Reservations.Update(reservation);
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await _dbContext.Reservations.AddAsync(reservation, cancellationToken);
    }

    public async Task<IReadOnlyList<Reservation>> GetExpiredPendingAsync(DateTime now, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reservations
            .Where(x => x.ExpirationDate < now && x.ReservationStatus == ReservationStatus.Pending)
            .ToListAsync(cancellationToken);
    }
}
