using ModularReservations.Reservations.Domain.Aggregates;

namespace ModularReservations.Reservations.Domain.Repositories;

public interface IScreeningRepository
{
     Task<Screening?> GetByIdAsync(Guid screeningId, CancellationToken cancellationToken = default);
     Task UpdateAsync(Screening screening, CancellationToken cancellationToken = default);
     Task AddAsync(Screening screening, CancellationToken cancellationToken = default);

}
