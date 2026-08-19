using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnterpriseLicenseSystem.Domain.Entities;

namespace EnterpriseLicenseSystem.Application.Assets.Queries;

public record AssetBriefDto
{
    public int Id { get; init; }
    public string Model { get; init; } = string.Empty;
    public string SerialNumber { get; init; } = string.Empty;
    public string? AssignedToUserId { get; init; }
    public bool IsAssigned => !string.IsNullOrEmpty(AssignedToUserId);

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Asset, AssetBriefDto>();
        }
    }
}
