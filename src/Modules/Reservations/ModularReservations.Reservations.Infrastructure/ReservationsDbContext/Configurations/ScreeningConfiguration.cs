using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModularReservations.Reservations.Domain.Aggregates;
using ModularReservations.Reservations.Infrastructure.ReservationsDbContext.Converts;

namespace ModularReservations.Reservations.Infrastructure.ReservationsDbContext.Configurations;

public class ScreeningConfiguration : IEntityTypeConfiguration<Screening>
{
    public void Configure(EntityTypeBuilder<Screening> builder)
    {
        builder.ToTable("Screenings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();
        builder.Ignore(x => x.Events);
        builder.Property(x => x.ScreeningDate)
            .HasConversion<UtcDateTimeConverter>();
        builder.OwnsMany(x => x.Seats, seat =>
        {
            seat.ToTable("Seats");
            seat.WithOwner().HasForeignKey("ScreeningId");
            seat.HasKey(s => s.Id);
            seat.Property(s => s.Id).ValueGeneratedNever();
            seat.OwnsOne(s => s.Position, p =>
            {
                p.Property(x => x.RowNumber).HasColumnName("RowNumber");
                p.Property(x => x.NumberInRow).HasColumnName("NumberInRow");
            });
            seat.Navigation(s => s.Position).IsRequired();
            seat.Property(s => s.IsReserved).IsConcurrencyToken();
        });
    }
}
