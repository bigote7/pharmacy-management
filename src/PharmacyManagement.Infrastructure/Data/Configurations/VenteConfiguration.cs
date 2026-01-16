using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyManagement.Domain.Entities;

namespace PharmacyManagement.Infrastructure.Data.Configurations;

public class VenteConfiguration : IEntityTypeConfiguration<Vente>
{
    public void Configure(EntityTypeBuilder<Vente> builder)
    {
        builder.HasKey(v => v.Id);
        
        builder.Property(v => v.NumeroFacture)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(v => v.MontantTotal)
            .HasColumnType("decimal(18,2)");
            
        builder.Property(v => v.MontantTVA)
            .HasColumnType("decimal(18,2)");
            
        builder.Property(v => v.Statut)
            .HasMaxLength(50)
            .HasDefaultValue("Normal");
            
        builder.HasOne(v => v.Client)
            .WithMany(c => c.Ventes)
            .HasForeignKey(v => v.ClientId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasOne(v => v.User)
            .WithMany()
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasMany(v => v.VenteDetails)
            .WithOne(vd => vd.Vente)
            .HasForeignKey(vd => vd.VenteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

