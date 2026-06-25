using Benefits.Application.Features.Employees.Common;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Benefits.Application.Features.Employees.Queries.GetEmployeeById
{
    public sealed class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeBasicInfoDto?>
    {
        private readonly IBenefitsDbContext _dbContext;

        public GetEmployeeByIdHandler(IBenefitsDbContext dbContext)
        {
            _dbContext = Guard.NotNull(dbContext);
        }

        public async Task<EmployeeBasicInfoDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Employees.Where(e => e.Id == request.Id)
                .Select(e => new EmployeeBasicInfoDto
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
