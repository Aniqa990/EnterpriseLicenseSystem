using EnterpriseLicenseSystem.Application.Common.Interfaces;
using EnterpriseLicenseSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands.CreateSoftwareLicenseCommand;

// The Command Request (Data payload passed into MediatR)
public record CreateSoftwareLicenseCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string LicenseKey { get; init; } = string.Empty;
    public int TotalSeats { get; init; }
    public DateTime ExpirationDate { get; init; }
}

// The Command Validator (FluentValidation pipeline)
public class CreateSoftwareLicenseCommandValidator : AbstractValidator<CreateSoftwareLicenseCommand>
{
    public CreateSoftwareLicenseCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(200)
            .NotEmpty()
            .WithMessage("License name is required.");

        RuleFor(v => v.LicenseKey)
            .MaximumLength(100)
            .NotEmpty()
            .WithMessage("License key is required.");

        RuleFor(v => v.TotalSeats)
            .GreaterThan(0)
            .WithMessage("Total seats must be greater than zero.");

        RuleFor(v => v.ExpirationDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Expiration date must be in the future.");
    }
}

// The Command Handler (Executes write operation)
public class CreateSoftwareLicenseCommandHandler : IRequestHandler<CreateSoftwareLicenseCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateSoftwareLicenseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateSoftwareLicenseCommand request, CancellationToken cancellationToken)
    {
        var entity = new SoftwareLicense
        {
            Name = request.Name,
            LicenseKey = request.LicenseKey,
            TotalSeats = request.TotalSeats,
            AllocatedSeats = 0,
            ExpirationDate = request.ExpirationDate
        };

        _context.SoftwareLicenses.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
