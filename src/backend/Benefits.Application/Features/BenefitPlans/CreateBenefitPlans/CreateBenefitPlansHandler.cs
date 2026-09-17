using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Benefits.Domain;
using MediatR;

namespace Benefits.Application.Features.BenefitPlans.CreateBenefitPlans
{
    public class CreateBenefitPlansHandler : IRequestHandler<CreateBenefitPlansCommand, int>
    {
        private readonly IBenefitsDbContext _dbContext;

        public CreateBenefitPlansHandler(IBenefitsDbContext dbContext)
        {
            _dbContext = Guard.NotNull(dbContext);
        }

        public async Task<int> Handle(CreateBenefitPlansCommand request, CancellationToken cancellationToken)
        {
            var benefitPlan = new BenefitPlan
            {
                Name = request.Name,
                Description = request.Description,
                BenefitTypeId = request.BenefitTypeId,
            };

            _dbContext.BenefitPlans.Add(benefitPlan);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return benefitPlan.Id;
        }
    }
}
