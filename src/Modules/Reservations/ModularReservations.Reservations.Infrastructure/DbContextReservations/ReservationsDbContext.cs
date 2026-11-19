using Microsoft.EntityFrameworkCore;

namespace ModularReservations.Reservations.Infrastructure.DbContextReservations;

public class ReservationsDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("reservations");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationsDbContext).Assembly);
    }
}
