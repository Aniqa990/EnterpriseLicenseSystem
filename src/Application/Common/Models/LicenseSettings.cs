using System.ComponentModel.DataAnnotations;

namespace EnterpriseLicenseSystem.Application.Common.Models;

// Options Pattern: bind a configuration section strongly instead of reading raw
// builder.Configuration["LicenseSettings:ExpirationWarningDays"] scattered through the code.
public class LicenseSettings
{
    public const string SectionName = "LicenseSettings";

    [Range(1, 365)]
    public int ExpirationWarningDays { get; init; } = 30;

    [Range(1, 100000)]
    public int MaxSeatsPerLicense { get; init; } = 500;
}
