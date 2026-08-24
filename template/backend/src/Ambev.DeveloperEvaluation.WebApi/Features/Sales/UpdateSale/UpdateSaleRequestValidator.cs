using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
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

            RuleForEach(s => s.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty();
                item.RuleFor(i => i.ProductName).MaximumLength(100);
                item.RuleFor(i => i.Quantity).GreaterThan(0).LessThanOrEqualTo(20).When(i => i.Quantity.HasValue);
                item.RuleFor(i => i.UnitPrice).GreaterThan(0).When(i => i.UnitPrice.HasValue);
            });
        });
    }
}