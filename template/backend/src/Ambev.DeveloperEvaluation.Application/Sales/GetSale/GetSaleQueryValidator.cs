using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Validator for GetSaleCommand that defines validation rules for sale creation command.
/// </summary>
public class GetSaleQueryValidator : AbstractValidator<GetSaleQuery>
{
    /// <summary>
    /// Initializes a new instance of the GetSaleQueryValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include only checking that the SaleNumber is not empty.
    /// </remarks>
    public GetSaleQueryValidator()
    {
        RuleFor(s => s.Id).NotEmpty();
    }
}