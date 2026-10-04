export interface EmployeeListItem {
  id: number;
  employeeNumber: string;
  firstName: string;
  lastName: string;
  email: string;
  hireDate: string;
  departmentName: string;

  userName?: string;
  hasUser?: boolean;
}