
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(r => r.CustomerId).NotEmpty();
        RuleFor(r => r.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(r => r.BranchId).NotEmpty();
        RuleFor(r => r.BranchName).NotEmpty().MaximumLength(100);
        RuleFor(r => r.Items).NotEmpty()
            .Must(items => items == null || items
                .GroupBy(i => i.ProductId)
                .All(g => g.Select(i => i.ProductName).Distinct(StringComparer.OrdinalIgnoreCase).Count() <= 1))
            .WithMessage("Items with the same ProductId cannot have different ProductNames.")
            .Must(items => items == null || items
                .GroupBy(i => i.ProductId)
                .All(g => g.Select(i => i.UnitPrice).Distinct().Count() <= 1))
            .WithMessage("Items with the same ProductId cannot have different UnitPrices.");

        RuleForEach(r => r.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.ProductName).NotEmpty().MaximumLength(100);
            item.RuleFor(i => i.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
            item.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
}