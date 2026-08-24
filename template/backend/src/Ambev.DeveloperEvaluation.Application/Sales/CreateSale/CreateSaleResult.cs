namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Represents the response returned after successfully creating a new sale.
/// </summary>
/// <remarks>
/// This response contains the unique identifier of the newly created sale,
/// which can be used for subsequent operations or reference.
/// </remarks>
public record CreateSaleResult(string SaleNumber, DateTime SaleDate, string CustomerId, string CustomerName, string BranchId, string BranchName, List<CreateSaleItemCommand> Items, decimal TotalAmount);
