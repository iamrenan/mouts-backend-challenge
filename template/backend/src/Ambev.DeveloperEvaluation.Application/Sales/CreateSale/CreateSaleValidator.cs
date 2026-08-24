
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleCommand that defines validation rules for sale creation command.
/// </summary>
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateSaleCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// 
    /// </remarks>
    public CreateSaleCommandValidator()
    {
        RuleFor(s => s.CustomerId).NotEmpty();
        RuleFor(s => s.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(s => s.BranchId).NotEmpty();
        RuleFor(s => s.BranchName).NotEmpty().MaximumLength(100);
        RuleFor(s => s.Items).NotEmpty()
            .Must(items => items == null || items
                .GroupBy(i => i.ProductId)
                .All(g => g.Select(i => i.ProductName).Distinct(StringComparer.OrdinalIgnoreCase).Count() <= 1))
            .WithMessage("Items with the same ProductId cannot have different ProductNames.")
            .Must(items => items == null || items
                .GroupBy(i => i.ProductId)
                .All(g => g.Select(i => i.UnitPrice).Distinct().Count() <= 1))
            .WithMessage("Items with the same ProductId cannot have different UnitPrices.");

        RuleForEach(s => s.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.ProductId).NotEmpty();
            items.RuleFor(i => i.Quantity).InclusiveBetween(1, 20);
            items.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
}