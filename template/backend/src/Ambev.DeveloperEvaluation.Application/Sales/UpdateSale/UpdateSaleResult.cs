
namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Represents the response returned after successfully updating a sale.
/// </summary>
public record UpdateSaleResult(
    Guid Id,
    string SaleNumber,
    DateTime SaleDate,
    Guid CustomerId,
    string CustomerName,
    Guid BranchId,
    string BranchName,
    List<UpdateSaleItemResult> Items,
    decimal TotalAmount,
    bool IsCancelled
);

public record UpdateSaleItemResult(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Discount,
    decimal Total,
    bool IsCancelled
);