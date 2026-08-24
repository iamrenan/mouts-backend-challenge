namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// API response model for UpdateSale operation
/// </summary>
public record UpdateSaleResponse
(
     Guid Id,
     string SaleNumber,
     DateTime SaleDate,
     Guid CustomerId,
     string CustomerName,
     Guid BranchId,
     string BranchName,
     List<UpdateSaleItemResponse> Items,
     decimal TotalAmount,
     bool IsCancelled
);

public record UpdateSaleItemResponse(
     Guid ProductId,
     string ProductName,
     int Quantity,
     decimal UnitPrice,
     decimal Total,
     decimal Discount,
     bool IsCancelled
);