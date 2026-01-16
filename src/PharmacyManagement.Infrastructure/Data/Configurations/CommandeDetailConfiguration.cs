using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyManagement.Domain.Entities;

namespace PharmacyManagement.Infrastructure.Data.Configurations;

public class CommandeDetailConfiguration : IEntityTypeConfiguration<CommandeDetail>
{
    public void Configure(EntityTypeBuilder<CommandeDetail> builder)
    {
        builder.HasKey(cd => cd.Id);
        
        builder.Property(cd => cd.PrixUnitaire)
            .HasColumnType("decimal(18,2)");
            
        builder.Property(cd => cd.SousTotal)
            .HasColumnType("decimal(18,2)");
            
        builder.HasOne(cd => cd.Medicament)
            .WithMany(m => m.CommandeDetails)
            .HasForeignKey(cd => cd.MedicamentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

