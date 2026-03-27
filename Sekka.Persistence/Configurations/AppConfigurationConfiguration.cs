using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class AppConfigurationConfiguration : IEntityTypeConfiguration<AppConfiguration>
{
    public void Configure(EntityTypeBuilder<AppConfiguration> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.ConfigKey).IsRequired().HasMaxLength(100);
        builder.Property(c => c.ConfigValue).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(200);
        builder.Property(c => c.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(c => c.ConfigKey).IsUnique();
    }
}
