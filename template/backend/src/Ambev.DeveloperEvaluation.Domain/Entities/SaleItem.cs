using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Total { get; private set; }
    public decimal Discount { get; private set; }
    public bool IsCancelled { get; private set; }

    public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (quantity > 20)
            throw new DomainException("Cannot sell more than 20 identical items.");

        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        CalculateDiscount();
    }

    internal void UpdateQuantity(int quantity)
    {
        if (quantity > 20)
            throw new DomainException("Cannot sell more than 20 identical items.");

        Quantity = quantity;
        CalculateDiscount();
    }

    internal void CalculateDiscount(int? totalProductQuantity = null)
    {
        var effectiveQuantity = totalProductQuantity ?? Quantity;
        var discountRate = effectiveQuantity switch
        {
            >= 10 and <= 20 => 0.20m,
            >= 4 and < 10 => 0.10m,
            _ => 0m
        };

        Discount = Math.Round(Quantity * UnitPrice * discountRate, 2);
        Total = Math.Round((Quantity * UnitPrice) - Discount, 2);
    }

    internal void Cancel()
    {
        IsCancelled = true;
        Discount = 0m;
        Total = 0m;
    }
}