using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class CommunityBlacklistConfiguration : IEntityTypeConfiguration<CommunityBlacklist>
{
    public void Configure(EntityTypeBuilder<CommunityBlacklist> builder)
    {
        builder.HasKey(b => b.PhoneNumber);
        builder.Property(b => b.PhoneNumber).HasMaxLength(20);
        builder.Property(b => b.SeverityScore).HasPrecision(5, 2);
    }
}
