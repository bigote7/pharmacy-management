using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyManagement.Domain.Entities;

namespace PharmacyManagement.Infrastructure.Data.Configurations;

public class PrescriptionDetailConfiguration : IEntityTypeConfiguration<PrescriptionDetail>
{
    public void Configure(EntityTypeBuilder<PrescriptionDetail> builder)
    {
        builder.HasKey(pd => pd.Id);
        
        builder.Property(pd => pd.Posologie)
            .HasMaxLength(500);
            
        builder.HasOne(pd => pd.Medicament)
            .WithMany(m => m.PrescriptionDetails)
            .HasForeignKey(pd => pd.MedicamentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

