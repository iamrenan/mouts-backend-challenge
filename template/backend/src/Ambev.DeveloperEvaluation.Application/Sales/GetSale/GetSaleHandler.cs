using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Handler for processing GetSaleQuery requests
/// </summary>
public class GetSaleHandler(ISaleRepository saleRepository, IMapper mapper) : IRequestHandler<GetSaleQuery, GetSaleResult>
{

    /// <summary>
    /// Handles the GetSaleQuery request
    /// </summary>
    /// <param name="query">The GetSale query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deleted sale details</returns>
    public async Task<GetSaleResult> Handle(GetSaleQuery query, CancellationToken cancellationToken)
    {
        var validator = new GetSaleQueryValidator();
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await saleRepository.GetByIdAsync(query.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {query.Id} not found");

        return mapper.Map<GetSaleResult>(sale);
    }
}
