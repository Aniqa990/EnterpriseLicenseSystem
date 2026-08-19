using EnterpriseLicenseSystem.Application.Common.Exceptions;
using EnterpriseLicenseSystem.Application.Common.Interfaces;
using EnterpriseLicenseSystem.Application.Common.Models;
using EnterpriseLicenseSystem.Domain.Events;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands.RevokeLicenseCommand;

public record RevokeLicenseCommand(int LicenseId, string UserId) : IRequest<Result<bool>>;

public class RevokeLicenseCommandValidator : AbstractValidator<RevokeLicenseCommand>
{
    public RevokeLicenseCommandValidator()
    {
        RuleFor(v => v.LicenseId).GreaterThan(0).WithMessage("Valid license ID required.");
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User ID is required.");
    }
}

public class RevokeLicenseCommandHandler : IRequestHandler<RevokeLicenseCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public RevokeLicenseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(RevokeLicenseCommand request, CancellationToken cancellationToken)
    {
        var license = await _context.SoftwareLicenses
            .Include(l => l.Assignments)
            .FirstOrDefaultAsync(l => l.Id == request.LicenseId, cancellationToken);

        if (license is null)
        {
            return Result<bool>.Failure($"License {request.LicenseId} was not found.");
        }

        var assignment = license.Assignments.FirstOrDefault(a => a.UserId == request.UserId);

        if (assignment is null)
        {
            return Result<bool>.Failure("This license is not currently assigned to that user.");
        }

        license.Assignments.Remove(assignment);
        _context.LicenseAssignments.Remove(assignment);
        license.AllocatedSeats = Math.Max(0, license.AllocatedSeats - 1);

        license.AddDomainEvent(new LicenseRevokedEvent(assignment));

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("The license was modified by another user. Please refresh and try again.", ex);
        }

        return Result<bool>.Success(true);
    }
}
