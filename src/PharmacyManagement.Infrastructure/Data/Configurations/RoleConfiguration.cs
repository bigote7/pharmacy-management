using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyManagement.Domain.Entities;

namespace PharmacyManagement.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Nom)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasIndex(r => r.Nom)
            .IsUnique();
            
        builder.Property(r => r.Description)
            .HasMaxLength(500);
    }
}

