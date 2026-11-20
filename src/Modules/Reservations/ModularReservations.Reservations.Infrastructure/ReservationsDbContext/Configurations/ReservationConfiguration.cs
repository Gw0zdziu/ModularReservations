using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModularReservations.Reservations.Domain.Aggregates;
using ModularReservations.Reservations.Infrastructure.ReservationsDbContext.Converts;

namespace ModularReservations.Reservations.Infrastructure.ReservationsDbContext.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{

    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();
        builder.Ignore(x => x.Events);
        builder.Property(x => x.ExpirationDate)
            .HasConversion<UtcDateTimeConverter>();
        builder.Property(x => x.ReservationDate)
            .HasConversion<UtcDateTimeConverter>();
        builder.Property(x => x.ReservationStatus)
            .HasMaxLength(20)
            .HasConversion<string>();

    }
}
