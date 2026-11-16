using ModularReservations.Reservations.Domain.Aggregates;

namespace ModularReservations.Reservations.Domain.Repositories;

public interface IReservationRepository
{
     Task<Reservation?> GetByIdAsync(Guid reservationId, CancellationToken cancellationToken = default);
     Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default);
     Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
     Task<IReadOnlyList<Reservation>> GetExpiredPendingAsync(DateTime now, CancellationToken cancellationToken = default);
}
