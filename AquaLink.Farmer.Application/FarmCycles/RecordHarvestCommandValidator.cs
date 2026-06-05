using FluentValidation;

namespace AquaLink.Farmer.Application.FarmCycles;

public class RecordHarvestCommandValidator
    : AbstractValidator<RecordHarvestCommand>
{
    public RecordHarvestCommandValidator()
    {
        RuleFor(x => x.FarmCycleId)
            .NotEmpty()
            .WithMessage("Farm cycle ID is required.");

        RuleFor(x => x.HarvestedWeightKg)
            .GreaterThan(0)
            .WithMessage("Harvested weight must be greater than zero.")
            .LessThanOrEqualTo(10000)
            .WithMessage("Harvested weight seems unusually high. Please verify.");

        RuleFor(x => x.SalePricePerKg)
            .GreaterThan(0)
            .WithMessage("Sale price must be greater than zero.")
            .LessThanOrEqualTo(100000)
            .WithMessage("Sale price seems unusually high. Please verify.");

        RuleFor(x => x.HarvestedAt)
            .NotEmpty()
            .WithMessage("Harvest date is required.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Harvest date cannot be in the future.");
    }
}