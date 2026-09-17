using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.BenefitTypes.Queries.GetBenefitPlansByType
{
    public sealed class GetBenefitPlansByTypeHandler : IRequestHandler<GetBenefitPlansByTypeQuery, IReadOnlyList<BenefitPlanListItemDto>>
    {
        private readonly IBenefitsDbContext _dbContext;

        public GetBenefitPlansByTypeHandler(IBenefitsDbContext dbContext)
        {
            _dbContext = Guard.NotNull(dbContext);
        }

        public async Task<IReadOnlyList<BenefitPlanListItemDto>> Handle(GetBenefitPlansByTypeQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.BenefitPlans
                .AsNoTracking()
                .Where(bp => bp.BenefitTypeId == request.Id && bp.IsActive)
                .OrderBy(bp => bp.Name)
                .Select(bp => new BenefitPlanListItemDto
                (
                    bp.Id,
                    bp.Name,
                    bp.Description
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
