using EnterpriseLicenseSystem.Application.Common.Interfaces;
using EnterpriseLicenseSystem.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace EnterpriseLicenseSystem.Application.Assets.Commands.UnassignAsset;

public record UnassignAssetCommand(int AssetId) : IRequest<Result<bool>>;

public class UnassignAssetCommandValidator : AbstractValidator<UnassignAssetCommand>
{
    public UnassignAssetCommandValidator()
    {
        RuleFor(v => v.AssetId).GreaterThan(0).WithMessage("Valid asset ID required.");
    }
}

public class UnassignAssetCommandHandler : IRequestHandler<UnassignAssetCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UnassignAssetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UnassignAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _context.Assets.FindAsync(new object[] { request.AssetId }, cancellationToken);

        if (asset is null)
        {
            return Result<bool>.Failure($"Asset {request.AssetId} was not found.");
        }

        if (asset.AssignedToUserId is null)
        {
            return Result<bool>.Failure("Asset is not currently assigned to anyone.");
        }

        asset.AssignedToUserId = null;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
