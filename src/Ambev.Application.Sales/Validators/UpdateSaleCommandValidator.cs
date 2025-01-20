using Ambev.Application.Sales.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.Application.Sales.Validators
{
    public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
    {
        public UpdateSaleCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .WithMessage("Sale ID is required.");

            RuleFor(c => c.Branch)
                .MaximumLength(100)
                .WithMessage("Branch must not exceed 100 characters.");

            RuleForEach(c => c.Items)
                .SetValidator(new UpdateSaleItemCommandValidator());
        }
    }
}
