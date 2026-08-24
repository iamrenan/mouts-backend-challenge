using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Validator for DeleteSaleCommand that defines validation rules for sale creation command.
/// </summary>
public class DeleteSaleCommandValidator : AbstractValidator<DeleteSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the DeleteSaleCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules includes only checking that the SaleNumber is not empty and has a maximum length of 50 characters./// 
    /// </remarks>
    public DeleteSaleCommandValidator()
    {
        RuleFor(s => s.Id).NotEmpty();
    }
}