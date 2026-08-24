using Ambev.DeveloperEvaluation.Common.Validation;
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
    public string CustomerId { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public string BranchId { get; private set; } = string.Empty;
    public string BranchName { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    private readonly List<SaleItem> _items = new();
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();
    public bool IsCancelled { get; private set; }
    public Sale(string saleNumber, string customerId, string customerName,
                string branchId, string branchName)
    {
        SaleNumber = saleNumber;
        SaleDate = DateTime.UtcNow;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
    }

    public void AddItem(string productId, string productName, int quantity, decimal unitPrice)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot add items to a cancelled sale.");

        var existing = _items.FirstOrDefault(i => i.ProductId == productId && !i.IsCancelled);
        var newQuantity = (existing?.Quantity ?? 0) + quantity;

        if (newQuantity > 20)
            throw new DomainException($"Cannot sell more than 20 units of product {productId}.");

        if (existing is not null)
            existing.UpdateQuantity(newQuantity);
        else
            _items.Add(new SaleItem(productId, productName, quantity, unitPrice));

        RecalculateTotal();
    }

    public void CancelItem(string productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId && !i.IsCancelled)
            ?? throw new DomainException($"Product {productId} not found in this sale.");

        item.Cancel();
        RecalculateTotal();
    }

    public void Cancel()
    {
        if (IsCancelled) return;

        IsCancelled = true;
        foreach (var item in _items.Where(i => !i.IsCancelled))
            item.Cancel();
    }

    private void RecalculateTotal()
    {
        foreach (var item in _items.Where(i => !i.IsCancelled))
            item.CalculateDiscount();

        TotalAmount = _items.Where(i => !i.IsCancelled).Sum(i => i.Total);
    }
}
