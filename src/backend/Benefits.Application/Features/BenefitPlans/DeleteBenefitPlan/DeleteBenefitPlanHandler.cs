using Benefits.Application.Exceptions;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.BenefitPlans.DeleteBenefitPlan
{
    public sealed class DeleteBenefitPlanHandler : IRequestHandler<DeleteBenefitPlanCommand>
    {
        private readonly IBenefitsDbContext _dbContext;

        public DeleteBenefitPlanHandler(IBenefitsDbContext dbContext)
        {
            _dbContext = Guard.NotNull(dbContext);
        }

        public async Task Handle(DeleteBenefitPlanCommand command, CancellationToken cancellationToken)
        {
            var benefitPlan = await _dbContext.BenefitPlans.FirstOrDefaultAsync(bp => bp.Id == command.Id, cancellationToken)
                ?? throw new NotFoundException($"Benefit Plan with Id {command.Id} was not found.");

            benefitPlan.Deactivate();

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
