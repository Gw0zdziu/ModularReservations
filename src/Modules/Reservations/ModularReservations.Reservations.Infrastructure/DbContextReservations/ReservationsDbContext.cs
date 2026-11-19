using Microsoft.EntityFrameworkCore;
using ModularReservations.Reservations.Domain.Aggregates;

namespace ModularReservations.Reservations.Infrastructure.DbContextReservations;

public class ReservationsDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : base(options)
    {

    }

    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Screening> Screenings => Set<Screening>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("reservations");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationsDbContext).Assembly);
    }
}
