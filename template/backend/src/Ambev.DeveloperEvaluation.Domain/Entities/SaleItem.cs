using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; private set; }
    public string ProductId { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Total { get; private set; }
    public decimal Discount { get; private set; }
    public bool IsCancelled { get; private set; }

    public SaleItem(string productId, string productName, int quantity, decimal unitPrice)
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

    internal void CalculateDiscount()
    {
        var discountRate = Quantity switch
        {
            >= 10 and <= 20 => 0.20m,
            >= 4 and < 10 => 0.10m,
            _ => 0m
        };

        Discount = Quantity * UnitPrice * discountRate;
        Total = (Quantity * UnitPrice) - Discount;
    }

    internal void Cancel() => IsCancelled = true;
}