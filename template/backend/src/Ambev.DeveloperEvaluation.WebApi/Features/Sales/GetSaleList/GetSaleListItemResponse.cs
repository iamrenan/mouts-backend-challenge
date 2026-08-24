namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleList;

/// <summary>
/// API response model for GetSaleList operation
/// </summary>
public record GetSaleListItemResponse(
    Guid Id,
    string SaleNumber,
    DateTime SaleDate,
    string CustomerName,
    string BranchName,
    decimal TotalAmount,
    bool IsCancelled,
    int ItemCount
);
