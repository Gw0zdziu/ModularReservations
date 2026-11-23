using Microsoft.EntityFrameworkCore;
using ModularReservations.Reservations.Domain.Aggregates;
using ModularReservations.Reservations.Domain.Interfaces.Repositories;
using ModularReservations.Reservations.Infrastructure.DbContextReservations;

namespace ModularReservations.Reservations.Infrastructure.Repositories;

public class ScreeningRepository : IScreeningRepository
{
    private readonly ReservationsDbContext _dbContext;

    public ScreeningRepository(ReservationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Screening?> GetByIdAsync(Guid screeningId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Screenings
            .Where(x => x.Id == screeningId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public void Update(Screening screening)
    {
        _dbContext.Update(screening);
    }

    public async Task AddAsync(Screening screening, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(screening, cancellationToken);
    }
}
