using FluentValidation;

namespace AquaLink.Farmer.Application.FarmCycles;

public class CreateFarmCycleCommandValidator
    : AbstractValidator<CreateFarmCycleCommand>
{
    public CreateFarmCycleCommandValidator()
    {
        RuleFor(x => x.FarmerId)
            .NotEmpty()
            .WithMessage("Farmer ID is required.");

        RuleFor(x => x.Species)
            .NotEmpty()
            .WithMessage("Species is required.")
            .MaximumLength(100)
            .WithMessage("Species must not exceed 100 characters.");

        RuleFor(x => x.StockedQuantity)
            .GreaterThan(0)
            .WithMessage("Stocked quantity must be greater than zero.");

        RuleFor(x => x.PondSizeSqm)
            .GreaterThan(0)
            .WithMessage("Pond size must be greater than zero.");

        RuleFor(x => x.StockedAt)
            .NotEmpty()
            .WithMessage("Stocking date is required.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Stocking date cannot be in the future.");

        RuleFor(x => x.ExpectedHarvestAt)
            .GreaterThan(x => x.StockedAt)
            .WithMessage("Expected harvest date must be after stocking date.")
            .When(x => x.ExpectedHarvestAt.HasValue);
    }
}