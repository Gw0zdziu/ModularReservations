using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ModularReservations.Shared.Infrastructure.Messaging.Outbox;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(x => x.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();
        builder.Property(p => p.Content)
            .IsRequired();
        builder.Property(p => p.Type)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(p => p.OccurredOn)
            .IsRequired();
        builder.HasIndex(x => new { x.ProcessedOn, x.OccurredOn });

    }
}
