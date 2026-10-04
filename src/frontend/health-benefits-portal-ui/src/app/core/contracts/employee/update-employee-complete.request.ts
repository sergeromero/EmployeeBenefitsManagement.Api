import { UpdateEmployeeRequest } from "./update-employee.request";
import { UpdateUserRequest } from "./update-user.request";


export interface UpdateEmployeeCompleteRequest {
    id: number;
    employee: UpdateEmployeeRequest;
    user: UpdateUserRequest;
    role: string;
}
