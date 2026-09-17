using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.BenefitPlans.Queries.GetBenefitPlans
{
    public sealed class GetBenefitPlansHandler : IRequestHandler<GetBenefitPlansQuery, IReadOnlyList<BenefitPlanListItemDto>>
    {
        private readonly IBenefitsDbContext _dbContext;

        public GetBenefitPlansHandler(IBenefitsDbContext dbContext)
        {
            _dbContext = Guard.NotNull(dbContext);
        }

        public async Task<IReadOnlyList<BenefitPlanListItemDto>> Handle(GetBenefitPlansQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.BenefitPlans
                .AsNoTracking()
                .Where(bp => bp.IsActive)
                .OrderBy(bp => bp.BenefitType.Name)
                .ThenBy(bp => bp.Name)
                .Select(bp => new BenefitPlanListItemDto(
                    bp.Id,
                    bp.Name,
                    new BenefitTypeDto(
                        bp.BenefitType.Id,
                        bp.BenefitType.Name)))
                .ToListAsync(cancellationToken);
        }
    }
}
