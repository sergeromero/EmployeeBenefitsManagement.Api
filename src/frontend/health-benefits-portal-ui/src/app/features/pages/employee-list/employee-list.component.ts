import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table'
import { MatPaginatorModule } from '@angular/material/paginator'
import { MatSortModule } from '@angular/material/sort'
import { MatIconModule } from '@angular/material/icon'
import { MatButtonModule } from '@angular/material/button'
import { EmployeesApiService } from '@core/api/employees/employees-api.service';
import { EmployeeListItem } from '@core/contracts/models/employee-list-item.model';
import { PagedResult } from '@core/contracts/models/paged-result.model';
import { AuthService } from '@core/auth/application/auth.service';

@Component({
  imports: [MatTableModule, MatPaginatorModule, MatSortModule, MatIconModule, MatButtonModule],
  selector: 'app-employee-list',
  styleUrl: './employee-list.component.scss',
  templateUrl: './employee-list.component.html',
})
export class EmployeeListComponent {
  private api = inject(EmployeesApiService);
  private authService = inject(AuthService);
  private router = inject(Router);

  employees = signal<EmployeeListItem[]>([]);
  totalCount = signal(0);
  page = signal(1);
  pageSize = signal(10);
  sortBy = signal("lastName");
  sortDirection = signal<"asc" | "desc">("asc");

  isAdmin = this.authService.hasRole("Administrator");
  isHR = this.authService.hasRole("HR");

  canEdit = this.isAdmin || this.isHR;

  displayedColumns = this.buildColumns();

  constructor() {
    this.load();
  }

  private buildColumns(): string[] {
    const base = ['employeeNumber', 'firstName', 'lastName', 'email', 'hireDate', 'departmentName'];

    if (this.isAdmin) {
      base.push('userName');
    } else {
      base.push('hasUser');
    }

    if (this.canEdit) {
      base.push('actions');
    }

    return base;
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

    editEmployee(employee: EmployeeListItem) {
    this.router.navigate(['/employees', employee.id, 'edit']);
  }
}
