using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Validator for UpdateSaleCommand that defines validation rules for sale creation command.
/// </summary>
public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the UpdateSaleCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// 
    /// </remarks>
    public UpdateSaleCommandValidator()
    {
        RuleFor(s => s.Id).NotEmpty();
        RuleFor(s => s.CustomerName).MaximumLength(100);
        RuleFor(s => s.BranchName).MaximumLength(100);
        When(s => s.Items != null, () =>
        {
            RuleFor(s => s.Items)
                .Must(items => items == null || items
                    .GroupBy(i => i.ProductId)
                    .All(g => g.Where(i => !string.IsNullOrEmpty(i.ProductName)).Select(i => i.ProductName).Distinct(StringComparer.OrdinalIgnoreCase).Count() <= 1))
                .WithMessage("Items with the same ProductId cannot have different ProductNames.")
                .Must(items => items == null || items
                    .GroupBy(i => i.ProductId)
                    .All(g => g.Where(i => i.UnitPrice.HasValue).Select(i => i.UnitPrice!.Value).Distinct().Count() <= 1))
                .WithMessage("Items with the same ProductId cannot have different UnitPrices.");

            RuleForEach(s => s.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.ProductId).NotEmpty();
                items.RuleFor(i => i.Quantity).InclusiveBetween(1, 20).When(i => i.Quantity.HasValue);
                items.RuleFor(i => i.UnitPrice).GreaterThan(0).When(i => i.UnitPrice.HasValue);
            });
        });
    }
}