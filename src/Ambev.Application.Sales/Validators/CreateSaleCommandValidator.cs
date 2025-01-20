using Ambev.Application.Sales.Commands;
using FluentValidation;

namespace Ambev.Application.Sales.Validators
{
    public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
    {
        public CreateSaleCommandValidator()
        {
            RuleFor(c => c.CustomerId)
                .NotEmpty()
                .WithMessage("CustomerId is required.");

            RuleFor(c => c.SaleDate)
                .NotEmpty()
                .WithMessage("SaleDate is required.")
                .GreaterThan(DateTime.MinValue)
                .WithMessage("SaleDate must be a valid date.");

            RuleForEach(c => c.Items)
                .SetValidator(new CreateSaleItemCommandValidator());
        }
    }
}
