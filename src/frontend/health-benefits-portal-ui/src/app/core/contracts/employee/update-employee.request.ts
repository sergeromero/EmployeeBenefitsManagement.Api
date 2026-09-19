export interface UpdateEmployeeRequest {
    id: number
    employeeId: string;
    firstName: string;
    lastName: string;
    email: string;
    hireDate: Date
    departmentId: number;
}