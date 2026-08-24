using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Handler for processing DeleteSaleCommand requests
/// </summary>
public class DeleteSaleHandler(ISaleRepository saleRepository, IMapper mapper) : IRequestHandler<DeleteSaleCommand, DeleteSaleResult>
{

    /// <summary>
    /// Handles the DeleteSaleCommand request
    /// </summary>
    /// <param name="request">The DeleteSale request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deleted sale details</returns>
    public async Task<DeleteSaleResult> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        bool isDeleted = await saleRepository.DeleteAsync(request.Id, cancellationToken);
        if (!isDeleted)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found.");

        return new DeleteSaleResult(true);
    }
}
