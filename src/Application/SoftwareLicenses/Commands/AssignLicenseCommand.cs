using System.Data;
using EnterpriseLicenseSystem.Application.Common.Interfaces;
using EnterpriseLicenseSystem.Application.Common.Exceptions;
using EnterpriseLicenseSystem.Domain.Entities;
using EnterpriseLicenseSystem.Domain.Events;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands.AssignLicenseCommand;

public record AssignLicenseCommand : IRequest<bool>
{
    public int LicenseId { get; init; }
    public string UserId { get; init; } = string.Empty;
}

public class AssignLicenseCommandHandler : IRequestHandler<AssignLicenseCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public AssignLicenseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(AssignLicenseCommand request, CancellationToken cancellationToken)
    {
        var license = await _context.SoftwareLicenses
            .Include(l => l.Assignments)
            .FirstOrDefaultAsync(l => l.Id == request.LicenseId, cancellationToken);

        if (license == null)
            throw new NotFoundException(nameof(SoftwareLicense), request.LicenseId.ToString());

        // Business Rule: Check Seat Availability
        if (license.AllocatedSeats >= license.TotalSeats)
        {
            throw new InvalidOperationException("No available seats left for this license.");
        }

        // Business Rule: Prevent duplicate assignment
        if (license.Assignments.Any(a => a.UserId == request.UserId))
        {
            throw new InvalidOperationException("License is already assigned to this user.");
        }

        var assignment = new LicenseAssignment
        {
            SoftwareLicenseId = license.Id,
            UserId = request.UserId,
            AssignedAt = DateTime.UtcNow
        };

        license.AllocatedSeats++;
        _context.LicenseAssignments.Add(assignment);

        // Domain Events Pattern: decouple the write (seat allocation) from side effects
        // (notification/audit) — handled asynchronously by LicenseAssignedEventHandler.
        license.AddDomainEvent(new LicenseAssignedEvent(assignment));

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("The license was modified by another user. Please refresh and try again.", ex);
        }

        return true;
    }
}
