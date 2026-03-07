using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sekka.Persistence.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Entities.Driver>
{
    public void Configure(EntityTypeBuilder<Entities.Driver> builder)
    {
        builder.Property(d => d.Name).IsRequired().HasMaxLength(100);
        builder.Property(d => d.ProfileImageUrl).HasMaxLength(500);
        builder.Property(d => d.LicenseImageUrl).HasMaxLength(500);
        builder.Property(d => d.CashOnHand).HasPrecision(18, 2);
        builder.Property(d => d.CashAlertThreshold).HasPrecision(18, 2);

        builder.HasIndex(d => d.PhoneNumber).IsUnique();
        builder.HasIndex(d => d.IsOnline);
        builder.HasIndex(d => d.IsActive);

        builder.HasOne(d => d.Preferences)
            .WithOne(p => p.Driver)
            .HasForeignKey<Entities.DriverPreferences>(p => p.DriverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.NotificationChannelPreferences)
            .WithOne(n => n.Driver)
            .HasForeignKey(n => n.DriverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.ActiveSessions)
            .WithOne(s => s.Driver)
            .HasForeignKey(s => s.DriverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.AccountDeletionRequests)
            .WithOne(r => r.Driver)
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.UserConsents)
            .WithOne(c => c.Driver)
            .HasForeignKey(c => c.DriverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.DataDeletionRequests)
            .WithOne(r => r.Driver)
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
