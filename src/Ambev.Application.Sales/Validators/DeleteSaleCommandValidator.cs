using Ambev.Application.Sales.Commands;
using FluentValidation;

namespace Ambev.Application.Sales.Validators
{
    public class DeleteSaleCommandValidator : AbstractValidator<DeleteSaleCommand>
    {
        public DeleteSaleCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .WithMessage("Sale ID is required.")
                .NotEqual(Guid.Empty)
                .WithMessage("Sale ID must be a valid GUID.");
        }
    }
}
