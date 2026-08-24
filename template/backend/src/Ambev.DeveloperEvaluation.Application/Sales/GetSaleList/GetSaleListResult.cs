namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleList;

public record GetSaleListItemResult(
    Guid Id,
    string SaleNumber,
    DateTime SaleDate,
    string CustomerName,
    string BranchName,
    decimal TotalAmount,
    bool IsCancelled,
    int ItemCount
);
public record GetSaleListResult(
    List<GetSaleListItemResult> Sales,
    int TotalCount,
    int Page,
    int PageSize
);