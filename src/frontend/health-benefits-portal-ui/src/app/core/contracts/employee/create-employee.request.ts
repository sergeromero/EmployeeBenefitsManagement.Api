export interface CreateEmployeeRequest {
    employeeNumber: string;
    firstName: string;
    lastName: string;
    email: string;
    hireDate: Date
    departmentId: number
}