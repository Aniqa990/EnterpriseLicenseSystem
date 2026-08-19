using EnterpriseLicenseSystem.Application.Common.Interfaces;
using EnterpriseLicenseSystem.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EnterpriseLicenseSystem.Application.Assets.Commands.CreateAsset;

public record CreateAssetCommand : IRequest<int>
{
    public string Model { get; init; } = string.Empty;
    public string SerialNumber { get; init; } = string.Empty;
}

public class CreateAssetCommandValidator : AbstractValidator<CreateAssetCommand>
{
    public CreateAssetCommandValidator()
    {
        RuleFor(v => v.Model)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Asset model is required.");

        RuleFor(v => v.SerialNumber)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Serial number is required.");
    }
}

public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateAssetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        var entity = new Asset
        {
            Model = request.Model,
            SerialNumber = request.SerialNumber
        };

        _context.Assets.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
