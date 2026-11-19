using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);
        // Domain-generated Id: without this, EF would UPDATE instead of INSERT a booking added via the navigation.
        builder.Property(b => b.Id).ValueGeneratedNever();

        builder.Property(b => b.BookedBy).IsRequired();
        builder.Property(b => b.Attendees).IsRequired();
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20); // enum as text

        // Value object stored as columns of the Bookings table (no separate table).
        builder.ComplexProperty(b => b.Range, range =>
        {
            range.Property(r => r.StartUtc).HasColumnName("StartUtc").HasConversion<UtcDateTimeConverter>();
            range.Property(r => r.EndUtc).HasColumnName("EndUtc").HasConversion<UtcDateTimeConverter>();
        });
    }
}
