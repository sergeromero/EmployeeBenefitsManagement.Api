import { EmployeeDto } from "./employee.dto";

export interface EmployeeWithUserDto {
    employee: EmployeeDto;
    user: {
        userName: string;
        email: string;
        password: string;
    } | undefined;
    role: string;
}