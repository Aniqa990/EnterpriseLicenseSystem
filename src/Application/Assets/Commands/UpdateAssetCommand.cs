using EnterpriseLicenseSystem.Application.Common.Interfaces;
using Ardalis.GuardClauses;
using FluentValidation;
using MediatR;

namespace EnterpriseLicenseSystem.Application.Assets.Commands.UpdateAsset;

public record UpdateAssetCommand : IRequest
{
    public int Id { get; init; }
    public string Model { get; init; } = string.Empty;
    public string SerialNumber { get; init; } = string.Empty;
}

public class UpdateAssetCommandValidator : AbstractValidator<UpdateAssetCommand>
{
    public UpdateAssetCommandValidator()
    {
        RuleFor(v => v.Model).NotEmpty().MaximumLength(200);
        RuleFor(v => v.SerialNumber).NotEmpty().MaximumLength(100);
    }
}

public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAssetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Assets.FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Model = request.Model;
        entity.SerialNumber = request.SerialNumber;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
