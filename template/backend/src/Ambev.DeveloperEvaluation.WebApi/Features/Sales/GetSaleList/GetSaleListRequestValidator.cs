using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleList;

public class GetSaleListRequestValidator : AbstractValidator<GetSaleListRequest>
{
    public GetSaleListRequestValidator()
    {
        RuleFor(s => s.PageSize).GreaterThan(0);
        RuleFor(s => s.Page).GreaterThan(0);
    }
}