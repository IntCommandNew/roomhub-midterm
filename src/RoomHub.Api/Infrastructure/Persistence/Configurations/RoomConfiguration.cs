using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Infrastructure.Persistence.Configurations;

public sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever(); // Id comes from the domain

        builder.Property(r => r.Name).IsRequired();
        builder.Property(r => r.Capacity).IsRequired();

        // Aggregate -> children, through the private backing field _bookings.
        builder.HasMany(r => r.Bookings)
            .WithOne()
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(r => r.Bookings).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
