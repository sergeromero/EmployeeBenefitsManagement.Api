import { CreateEmployeeRequest } from "./create-employee.request";
import { CreateUserRequest } from "./create-user.request";

export interface CreateEmployeeWithUserRequest {
    employee: CreateEmployeeRequest;
    user: CreateUserRequest;
    role: string;
}