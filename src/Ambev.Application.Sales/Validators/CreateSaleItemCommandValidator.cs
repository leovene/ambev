using Ambev.Application.Sales.Commands;
using FluentValidation;

namespace Ambev.Application.Sales.Validators
{
    public class CreateSaleItemCommandValidator : AbstractValidator<CreateSaleItemCommand>
    {
        public CreateSaleItemCommandValidator()
        {
            RuleFor(i => i.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required.");

            RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(i => i.UnitPrice)
                .GreaterThan(0)
                .WithMessage("UnitPrice must be greater than 0.");

            RuleFor(i => i.Discount)
                .InclusiveBetween(0, 1)
                .WithMessage("Discount must be between 0 and 1.");
        }
    }
}
