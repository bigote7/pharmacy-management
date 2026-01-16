using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyManagement.Domain.Entities;

namespace PharmacyManagement.Infrastructure.Data.Configurations;

public class MedicamentConfiguration : IEntityTypeConfiguration<Medicament>
{
    public void Configure(EntityTypeBuilder<Medicament> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Nom)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(m => m.CodeBarre)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(m => m.Dosage)
            .HasMaxLength(100);
            
        builder.Property(m => m.Forme)
            .HasMaxLength(50);
            
        builder.Property(m => m.PrixAchat)
            .HasColumnType("decimal(18,2)");
            
        builder.Property(m => m.PrixVente)
            .HasColumnType("decimal(18,2)");
            
        builder.HasOne(m => m.Categorie)
            .WithMany(c => c.Medicaments)
            .HasForeignKey(m => m.CategorieId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

