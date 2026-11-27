using Microsoft.EntityFrameworkCore;
using ModularReservations.Reservations.Domain.Aggregates;
using ModularReservations.Shared.Infrastructure.Messaging.Outbox;

namespace ModularReservations.Reservations.Infrastructure.DbContextReservations;

public class ReservationsDbContext : DbContext
{
    public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : base(options)
    {

    }

    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Screening> Screenings => Set<Screening>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("reservations");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationsDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
    }
}
