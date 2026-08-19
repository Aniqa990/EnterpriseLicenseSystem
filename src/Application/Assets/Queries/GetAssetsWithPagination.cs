using EnterpriseLicenseSystem.Application.Common.Interfaces;
using EnterpriseLicenseSystem.Application.Common.Mappings;
using EnterpriseLicenseSystem.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace EnterpriseLicenseSystem.Application.Assets.Queries.GetAssetsWithPagination;

public record GetAssetsWithPaginationQuery : IRequest<PaginatedList<AssetBriefDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetAssetsWithPaginationQueryValidator : AbstractValidator<GetAssetsWithPaginationQuery>
{
    public GetAssetsWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}

public class GetAssetsWithPaginationQueryHandler : IRequestHandler<GetAssetsWithPaginationQuery, PaginatedList<AssetBriefDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAssetsWithPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<AssetBriefDto>> Handle(GetAssetsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.Assets
            .OrderBy(x => x.Model)
            .Select(x => new AssetBriefDto
            {
                Id = x.Id,
                Model = x.Model,
                SerialNumber = x.SerialNumber,
                AssignedToUserId = x.AssignedToUserId
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
