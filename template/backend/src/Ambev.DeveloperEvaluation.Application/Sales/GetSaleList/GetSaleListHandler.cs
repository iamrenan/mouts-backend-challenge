using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleList;

/// <summary>
/// Handler for processing GetSaleListQuery requests
/// </summary>
public class GetSaleListHandler(ISaleRepository saleRepository, IMapper mapper)
    : IRequestHandler<GetSaleListQuery, GetSaleListResult>
{

    /// <summary>
    /// Handles the GetSaleListQuery request
    /// </summary>
    /// <param name="request">The GetSaleList request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale list result</returns>
    public async Task<GetSaleListResult> Handle(GetSaleListQuery request, CancellationToken cancellationToken)
    {
        var validator = new GetSaleListQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sales = await saleRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var totalCount = await saleRepository.CountAsync(cancellationToken);
        var items = sales.Select(s => new GetSaleListItemResult(s.Id, s.SaleNumber, s.SaleDate, s.CustomerName, s.BranchName, s.TotalAmount, s.IsCancelled, s.Items.Count)).ToList();

        return new GetSaleListResult(items, totalCount, request.Page, request.PageSize);
    }
}
