using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;


/// <summary>
/// Represents a sale in the system with details about the transaction.
/// This entity follows domain-driven design principles and includes business rules validation.
/// </summary>
public class Sale : BaseEntity
{
    public string SaleNumber { get; private set; } = string.Empty;
    public DateTime SaleDate { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public Guid BranchId { get; private set; }
    public string BranchName { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    private readonly List<SaleItem> _items = new();
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();
    public bool IsCancelled { get; private set; }

    public Sale() { }

    public void Initialize()
    {
        SaleDate = DateTime.UtcNow;
        SaleNumber = $"SALE-{SaleDate:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
    }

    public void SetCustomer(Guid customerId, string customerName)
    {
        CustomerId = customerId;
        CustomerName = customerName;
    }

    public void SetBranch(Guid branchId, string branchName)
    {
        BranchId = branchId;
        BranchName = branchName;
    }

    public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot add items to a cancelled sale.");

        _items.Add(new SaleItem(productId, productName, quantity, unitPrice));
        RecalculateTotal();
    }

    public void UpdateItems(IEnumerable<(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice)> items)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot update items of a cancelled sale.");

        var itemList = items.ToList();
        var productIds = itemList.Select(x => x.ProductId).ToHashSet();

        // Items not in the payload are cancelled and shouldn't account for the total
        foreach (var existingItem in _items.Where(i => !i.IsCancelled && !productIds.Contains(i.ProductId)))
        {
            existingItem.Cancel();
        }

        foreach (var productId in productIds)
        {
            var existingValidProducts = _items.Where(i => i.ProductId == productId && !i.IsCancelled).ToList();
            var updatedProducts = itemList.Where(i => i.ProductId == productId).ToList();

            var summedQuantity = updatedProducts.Sum(i => i.Quantity);
            if (summedQuantity == 0)
            {
                foreach (var item in existingValidProducts)
                    item.Cancel();
                continue;
            }

            if (summedQuantity > 20)
                throw new DomainException($"Unable to sell more than 20 items per product for {productId}.");

            // Cancel previously active items for this product and add payload items
            foreach (var item in existingValidProducts)
                item.Cancel();

            foreach (var item in updatedProducts)
            {
                _items.Add(new SaleItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice));
            }
        }

        RecalculateTotal();
    }

    public void CancelItem(Guid productId)
    {
        var items = _items.Where(i => i.ProductId == productId && !i.IsCancelled).ToList();
        if (items.Count == 0)
            throw new DomainException($"Product {productId} not found in this sale.");

        foreach (var item in items)
            item.Cancel();

        RecalculateTotal();
    }

    public void Cancel()
    {
        if (IsCancelled)
            return;

        IsCancelled = true;
        foreach (var item in _items.Where(i => !i.IsCancelled))
            item.Cancel();

        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        var activeItems = _items.Where(i => !i.IsCancelled).ToList();
        var productQuantities = activeItems
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));

        foreach (var (productId, totalQty) in productQuantities)
        {
            if (totalQty > 20)
                throw new DomainException($"Cannot sell more than 20 units of product {productId}.");
        }

        foreach (var item in activeItems)
        {
            item.CalculateDiscount(productQuantities[item.ProductId]);
        }

        TotalAmount = activeItems.Sum(i => i.Total);
    }
}
