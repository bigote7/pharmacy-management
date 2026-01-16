using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyManagement.Domain.Entities;

namespace PharmacyManagement.Infrastructure.Data.Configurations;

public class CommandeConfiguration : IEntityTypeConfiguration<Commande>
{
    public void Configure(EntityTypeBuilder<Commande> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Statut)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(c => c.MontantTotal)
            .HasColumnType("decimal(18,2)");
            
        builder.HasOne(c => c.Fournisseur)
            .WithMany(f => f.Commandes)
            .HasForeignKey(c => c.FournisseurId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(c => c.CommandeDetails)
            .WithOne(cd => cd.Commande)
            .HasForeignKey(cd => cd.CommandeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

