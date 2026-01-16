using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyManagement.Domain.Entities;

namespace PharmacyManagement.Infrastructure.Data.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.NumeroLot)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.HasOne(s => s.Medicament)
            .WithMany(m => m.Stocks)
            .HasForeignKey(s => s.MedicamentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

