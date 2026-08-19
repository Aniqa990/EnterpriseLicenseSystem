using EnterpriseLicenseSystem.Application.Common.Interfaces;

namespace EnterpriseLicenseSystem.Application.Assets.Commands.DeleteAsset;

public record DeleteAssetCommand(int Id) : IRequest;

public class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteAssetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Assets.FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        // Physically calls Remove() — the SoftDeleteInterceptor rewrites this into
        // IsDeleted = true for Asset entities, so the row is retained for audit purposes.
        _context.Assets.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
