using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Benefits.Domain;
using MediatR;

namespace Benefits.Application.Features.BenefitTypes.CreateBenefitTypes
{
    public class CreateBenefitTypesHandler : IRequestHandler<CreateBenefitTypesCommand, int>
    {
        private readonly IBenefitsDbContext _dbContext;

        public CreateBenefitTypesHandler(IBenefitsDbContext dbContext)
        {
            _dbContext = Guard.NotNull(dbContext);
        }

        public async Task<int> Handle(CreateBenefitTypesCommand request, CancellationToken cancellationToken)
        {
            var benefitType = new BenefitType
            {
                Name = request.Name
            };

            _dbContext.BenefitTypes.Add(benefitType);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return benefitType.Id;
        }
    }
}
