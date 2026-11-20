using ModularReservations.Reservations.Domain.Aggregates;

namespace ModularReservations.Reservations.Domain.Interfaces.Repositories;

public interface IScreeningRepository
{
     Task<Screening?> GetByIdAsync(Guid screeningId, CancellationToken cancellationToken = default);
     void Update(Screening screening);
     Task AddAsync(Screening screening, CancellationToken cancellationToken = default);

}
