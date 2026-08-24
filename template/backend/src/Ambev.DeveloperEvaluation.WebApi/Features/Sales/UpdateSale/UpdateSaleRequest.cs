namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public record UpdateSaleRequest(Guid Id, Guid? CustomerId, string? CustomerName, Guid? BranchId, string? BranchName, List<UpdateSaleItemRequest>? Items);

public record UpdateSaleItemRequest(Guid ProductId, string? ProductName, int? Quantity, decimal? UnitPrice);