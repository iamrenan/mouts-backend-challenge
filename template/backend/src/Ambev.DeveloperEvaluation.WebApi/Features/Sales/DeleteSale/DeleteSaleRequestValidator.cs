using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

public class DeleteSaleRequestValidator : AbstractValidator<DeleteSaleRequest>
{
    public DeleteSaleRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}
