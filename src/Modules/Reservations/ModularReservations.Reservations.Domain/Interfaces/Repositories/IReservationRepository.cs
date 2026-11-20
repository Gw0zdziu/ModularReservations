using ModularReservations.Reservations.Domain.Aggregates;

namespace ModularReservations.Reservations.Domain.Interfaces.Repositories;

public interface IReservationRepository
{
     Task<Reservation?> GetByIdAsync(Guid reservationId, CancellationToken cancellationToken = default);
     void Update(Reservation reservation);
     Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
     Task<IReadOnlyList<Reservation>> GetExpiredPendingAsync(DateTime now, CancellationToken cancellationToken = default);
}
