using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests
/// </summary>
public class UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper) : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    /// <summary>
    /// Handles the UpdateSaleCommand request
    /// </summary>
    /// <param name="command">The UpdateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        if (command.CustomerId.HasValue || command.CustomerName != null)
            sale.SetCustomer(command.CustomerId ?? sale.CustomerId, command.CustomerName ?? sale.CustomerName);

        if (command.BranchId.HasValue || command.BranchName != null)
            sale.SetBranch(command.BranchId ?? sale.BranchId, command.BranchName ?? sale.BranchName);

        if (command.Items != null)
        {
            var payloadItems = command.Items
                .Where(i => i.Quantity.HasValue && i.UnitPrice.HasValue)
                .Select(i => (i.ProductId, i.ProductName ?? "Unknown", i.Quantity!.Value, i.UnitPrice!.Value));

            sale.UpdateItems(payloadItems);
        }

        await saleRepository.UpdateAsync(sale, cancellationToken);
        var result = mapper.Map<UpdateSaleResult>(sale);
        return result;
    }
}
