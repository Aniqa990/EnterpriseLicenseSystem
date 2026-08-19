using EnterpriseLicenseSystem.Application.Common.Interfaces;
using EnterpriseLicenseSystem.Application.Common.Models;
using EnterpriseLicenseSystem.Domain.Events;
using FluentValidation;
using MediatR;

namespace EnterpriseLicenseSystem.Application.Assets.Commands.AssignAsset;

public record AssignAssetCommand(int AssetId, string UserId) : IRequest<Result<bool>>;

public class AssignAssetCommandValidator : AbstractValidator<AssignAssetCommand>
{
    public AssignAssetCommandValidator()
    {
        RuleFor(v => v.AssetId).GreaterThan(0).WithMessage("Valid asset ID required.");
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User ID is required.");
    }
}

public class AssignAssetCommandHandler : IRequestHandler<AssignAssetCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public AssignAssetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(AssignAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _context.Assets.FindAsync(new object[] { request.AssetId }, cancellationToken);

        if (asset is null)
        {
            return Result<bool>.Failure($"Asset {request.AssetId} was not found.");
        }

        if (asset.AssignedToUserId == request.UserId)
        {
            return Result<bool>.Failure("Asset is already assigned to this user.");
        }

        asset.AssignedToUserId = request.UserId;
        asset.AddDomainEvent(new AssetAssignedEvent(asset));

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
