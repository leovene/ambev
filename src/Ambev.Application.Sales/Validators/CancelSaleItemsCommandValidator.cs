using Ambev.Application.Sales.Commands;
using FluentValidation;

namespace Ambev.Application.Sales.Validators
{
    public class CancelSaleItemsCommandValidator : AbstractValidator<CancelSaleItemsCommand>
    {
        public CancelSaleItemsCommandValidator()
        {
            RuleFor(c => c.SaleId)
                .NotEmpty()
                .WithMessage("Sale ID is required.")
                .NotEqual(Guid.Empty)
                .WithMessage("Sale ID must be a valid GUID.");

            RuleFor(c => c.ItemIds)
                .NotEmpty()
                .WithMessage("At least one item ID is required.")
                .Must(ids => ids.All(id => id != Guid.Empty))
                .WithMessage("All item IDs must be valid GUIDs.");
        }
    }
}
