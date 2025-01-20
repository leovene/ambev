using Ambev.Application.Sales.Commands;
using FluentValidation;

namespace Ambev.Application.Sales.Validators
{
    public class CancelSaleCommandValidator : AbstractValidator<CancelSaleCommand>
    {
        public CancelSaleCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .WithMessage("Sale ID is required.")
                .NotEqual(Guid.Empty)
                .WithMessage("Sale ID must be a valid GUID.");
        }
    }
}
