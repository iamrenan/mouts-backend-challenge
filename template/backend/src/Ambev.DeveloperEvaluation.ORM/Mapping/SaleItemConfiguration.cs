using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(s => s.SaleId).IsRequired().HasColumnType("uuid");
        builder.Property(s => s.ProductId).IsRequired().HasColumnType("uuid");
        builder.Property(s => s.ProductName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Quantity).IsRequired();
        builder.Property(s => s.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(s => s.IsCancelled).IsRequired();
        builder.Property(s => s.Discount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(s => s.Total).IsRequired().HasColumnType("decimal(18,2)");
    }
}
