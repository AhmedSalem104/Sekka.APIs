using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class CallerIdNoteConfiguration : IEntityTypeConfiguration<CallerIdNote>
{
    public void Configure(EntityTypeBuilder<CallerIdNote> builder)
    {
        builder.HasIndex(c => new { c.DriverId, c.PhoneNumber }).IsUnique();
        builder.Property(c => c.PhoneNumber).HasMaxLength(20);
        builder.Property(c => c.DisplayName).HasMaxLength(100);
        builder.Property(c => c.Note).HasMaxLength(500);
        builder.HasOne(c => c.Driver).WithMany(d => d.CallerIdNotes).HasForeignKey(c => c.DriverId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.Customer).WithMany(cu => cu.CallerIdNotes).HasForeignKey(c => c.CustomerId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(c => c.Partner).WithMany(p => p.CallerIdNotes).HasForeignKey(c => c.PartnerId).OnDelete(DeleteBehavior.SetNull);
    }
}
