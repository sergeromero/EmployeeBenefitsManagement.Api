import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import { ApiClientService } from "../base/api-client.service";
import { EmployeeDto } from "../../contracts/employee/employee.dto";
import { CreateEmployeeRequest } from "../../contracts/employee/create-employee.request";
import { UpdateEmployeeRequest } from "../../contracts/employee/update-employee.request";

@Injectable({
    providedIn: "root"
})
export class EmployeesApiService {
    private api = inject(ApiClientService);

    getAll(): Observable<EmployeeDto[]> {
        return this.api.get<EmployeeDto[]>('employees');
    }

    getById(id: number): Observable<EmployeeDto> {
        return this.api.get<EmployeeDto>(`employees/${id}`);
    }

    create(request: CreateEmployeeRequest): Observable<number> {
        return this.api.post<CreateEmployeeRequest, number>('employees', request);
    }

    update(request: UpdateEmployeeRequest): Observable<void> {
        return this.api.put<UpdateEmployeeRequest, void>(`employees/${request.id}`, request);
    }

    delete(id: number): Observable<void> {
        return this.api.delete<void>(`employees/${id}`);
    }
}