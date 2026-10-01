export interface EmployeeListItem {
  employeeNumber: string;
  firstName: string;
  lastName: string;
  email: string;
  hireDate: string;
  departmentName: string;

  userName?: string;
  hasUser?: boolean;
}