using EnterpriseLicenseSystem.Application.Common.Interfaces;
using EnterpriseLicenseSystem.Application.Common.Mappings;
using EnterpriseLicenseSystem.Application.Common.Models;
using EnterpriseLicenseSystem.Application.SoftwareLicenses.Queries;

namespace EnterpriseLicenseSystem.Application.SoftwareLicenses.Queries.GetSoftwareLicensesWithPagination;

public record GetSoftwareLicensesWithPaginationQuery : IRequest<PaginatedList<SoftwareLicenseBriefDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetSoftwareLicensesWithPaginationQueryHandler : IRequestHandler<GetSoftwareLicensesWithPaginationQuery, PaginatedList<SoftwareLicenseBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSoftwareLicensesWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<SoftwareLicenseBriefDto>> Handle(
        GetSoftwareLicensesWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.SoftwareLicenses
            .OrderBy(x => x.Name)
            .Select(x => new SoftwareLicenseBriefDto
            {
                Id = x.Id,
                Name = x.Name,
                LicenseKey = x.LicenseKey,
                TotalSeats = x.TotalSeats,
                AllocatedSeats = x.AllocatedSeats,
                ExpirationDate = x.ExpirationDate
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
