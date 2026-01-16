using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyManagement.Domain.Entities;

namespace PharmacyManagement.Infrastructure.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Nom)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(c => c.Prenom)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(c => c.Telephone)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(c => c.Email)
            .HasMaxLength(200);
            
        builder.Property(c => c.Adresse)
            .HasMaxLength(500);
    }
}

