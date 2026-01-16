using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyManagement.Domain.Entities;

namespace PharmacyManagement.Infrastructure.Data.Configurations;

public class FournisseurConfiguration : IEntityTypeConfiguration<Fournisseur>
{
    public void Configure(EntityTypeBuilder<Fournisseur> builder)
    {
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Nom)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(f => f.Telephone)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(f => f.Email)
            .HasMaxLength(200);
            
        builder.Property(f => f.Adresse)
            .HasMaxLength(500);
    }
}

