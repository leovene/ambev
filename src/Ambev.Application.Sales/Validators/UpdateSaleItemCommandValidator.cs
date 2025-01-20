using Ambev.Application.Sales.Commands;
using FluentValidation;

namespace Ambev.Application.Sales.Validators
{
    public class UpdateSaleItemCommandValidator : AbstractValidator<UpdateSaleItemCommand>
    {
        public UpdateSaleItemCommandValidator()
        {
            RuleFor(i => i.Id)
                .NotEmpty()
                .WithMessage("Item ID is required for update.");

            RuleFor(i => i.ProductId)
                .NotEmpty()
                .WithMessage("Product ID is required.");

            RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(i => i.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit Price must be greater than 0.");
        }
    }
}
