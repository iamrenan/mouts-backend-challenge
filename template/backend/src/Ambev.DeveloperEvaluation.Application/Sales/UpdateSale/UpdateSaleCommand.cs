using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public record UpdateSaleCommand(
    Guid Id,
    Guid? CustomerId,
    string? CustomerName,
    Guid? BranchId,
    string? BranchName,
    List<UpdateSaleItemCommand>? Items
) : IRequest<UpdateSaleResult>;

public record UpdateSaleItemCommand(
    Guid ProductId,
    string? ProductName,
    int? Quantity,
    decimal? UnitPrice
);