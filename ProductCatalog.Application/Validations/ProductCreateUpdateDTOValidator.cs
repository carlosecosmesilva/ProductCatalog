using FluentValidation;
using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Validations
{
    public class ProductCreateUpdateDTOValidator : AbstractValidator<ProductCreateUpdateDTO>
    {
        public ProductCreateUpdateDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório.")
                .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Estoque não pode ser negativo.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("O valor do produto não pode ser negativo.");
        }
    }
}
