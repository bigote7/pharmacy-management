using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyManagement.Domain.Entities;

namespace PharmacyManagement.Infrastructure.Data.Configurations;

public class VenteDetailConfiguration : IEntityTypeConfiguration<VenteDetail>
{
    public void Configure(EntityTypeBuilder<VenteDetail> builder)
    {
        builder.HasKey(vd => vd.Id);
        
        builder.Property(vd => vd.PrixUnitaire)
            .HasColumnType("decimal(18,2)");
            
        builder.Property(vd => vd.SousTotal)
            .HasColumnType("decimal(18,2)");
            
        builder.HasOne(vd => vd.Medicament)
            .WithMany(m => m.VenteDetails)
            .HasForeignKey(vd => vd.MedicamentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

