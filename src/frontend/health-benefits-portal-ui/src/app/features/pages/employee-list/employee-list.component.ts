import { Component, inject, signal } from '@angular/core';
import { MatTableModule } from '@angular/material/table'
import { MatPaginatorModule } from '@angular/material/paginator'
import { MatSortModule } from '@angular/material/sort'
import { EmployeesApiService } from '@core/api/employees/employees-api.service';
import { EmployeeListItem } from '@core/contracts/models/employee-list-item.model';
import { PagedResult } from '@core/contracts/models/paged-result.model';
import { AuthService } from '@core/auth/application/auth.service';

@Component({
  imports: [MatTableModule, MatPaginatorModule, MatSortModule],
  selector: 'app-employee-list',
  styleUrl: './employee-list.component.scss',
  templateUrl: './employee-list.component.html',
})
export class EmployeeListComponent {
  private api = inject(EmployeesApiService);
  private authService = inject(AuthService);

  employees = signal<EmployeeListItem[]>([]);
  totalCount = signal(0);
  page = signal(1);
  pageSize = signal(10);
  sortBy = signal("lastName");
  sortDirection = signal<"asc" | "desc">("asc");

  isAdmin = this.authService.hasRole("Administrator");

  displayedColumns = this.isAdmin
  ? ['employeeNumber', 'firstName', 'lastName', 'email', 'hireDate', 'departmentName', 'userName']
  : ['employeeNumber', 'firstName', 'lastName', 'email', 'hireDate', 'departmentName', 'hasUser'];

  constructor() {
    this.load();
  }

  load() {
    this.api.getAll({
      page: this.page(),
      pageSize: this.pageSize(),
      sortBy: this.sortBy(),
      sortDirection: this.sortDirection()
    }).subscribe((res: PagedResult<EmployeeListItem>) => {
      this.employees.set(res.items);
      this.totalCount.set(res.totalCount);
    });
  }

  onPageChange(event: any) {
    this.page.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.load();
  }

  onSortChange(event: any) {
    this.sortBy.set(event.active);
    this.sortDirection.set(event.direction || "asc");
    this.load();
  }
}
