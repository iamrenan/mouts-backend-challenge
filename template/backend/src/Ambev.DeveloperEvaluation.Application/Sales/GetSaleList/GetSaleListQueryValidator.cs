using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleList;

/// <summary>
/// Validator for GetSaleListQuery that defines validation rules for sale list retrieval query.
/// </summary>
public class GetSaleListQueryValidator : AbstractValidator<GetSaleListQuery>
{
    /// <summary>
    /// Initializes a new instance of the GetSaleListQueryValidator with defined validation rules.
    /// </summary>
    public GetSaleListQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}