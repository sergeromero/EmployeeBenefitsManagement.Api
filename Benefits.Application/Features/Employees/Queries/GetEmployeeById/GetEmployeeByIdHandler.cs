using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Benefits.Application.Features.Employees.Queries.GetEmployeeById
{
    public sealed class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, GetEmployeeByIdResponse?>
    {
        private readonly IBenefitsDbContext _dbContext;

        public GetEmployeeByIdHandler(IBenefitsDbContext dbContext)
        {
            _dbContext = Argument.NotNull(dbContext, nameof(dbContext));
        }

        public async Task<GetEmployeeByIdResponse?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Employees.Where(e => e.Id == request.Id)
                .Select(e => new GetEmployeeByIdResponse
                {
                    Id = e.Id,
                    EmployeeNumber = e.EmployeeNumber,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId,
                }).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
