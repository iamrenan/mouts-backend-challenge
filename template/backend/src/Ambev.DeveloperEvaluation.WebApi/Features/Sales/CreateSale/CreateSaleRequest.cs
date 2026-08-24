namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public record CreateSaleRequest(Guid CustomerId, string CustomerName, Guid BranchId, string BranchName, List<CreateSaleItemRequest> Items);

public record CreateSaleItemRequest(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);