using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyManagement.Domain.Entities;

namespace PharmacyManagement.Infrastructure.Data.Configurations;

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.NumeroPrescription)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(p => p.NomMedecin)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.HasOne(p => p.Client)
            .WithMany(c => c.Prescriptions)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(p => p.PrescriptionDetails)
            .WithOne(pd => pd.Prescription)
            .HasForeignKey(pd => pd.PrescriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

