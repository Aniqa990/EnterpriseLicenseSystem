namespace EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands.AssignLicenseCommand;
public class AssignLicenseCommandValidator : AbstractValidator<AssignLicenseCommand>
{
    public AssignLicenseCommandValidator()
    {
        RuleFor(v => v.LicenseId).GreaterThan(0).WithMessage("Valid license ID required.");
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User ID is required.");
    }
}
