import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import { APP_CONFIG } from "@core/config/app-config.token";
import { ApiClientService } from "../base/api-client.service";
import { EmployeeDto } from "../../contracts/employee/employee.dto";
import { CreateEmployeeRequest } from "../../contracts/employee/create-employee.request";
import { UpdateEmployeeRequest } from "../../contracts/employee/update-employee.request";

@Injectable({
    providedIn: "root"
})
export class EmployeesApiService {
    private api = inject(ApiClientService);
    private appConfig = inject(APP_CONFIG);

    getAll(): Observable<EmployeeDto[]> {
        return this.api.get<EmployeeDto[]>(`${this.appConfig.employeesUrl}`);
    }

    getById(id: number): Observable<EmployeeDto> {
        return this.api.get<EmployeeDto>(`${this.appConfig.employeesUrl}/${id}`);
    }

    create(request: CreateEmployeeRequest): Observable<number> {
        return this.api.post<CreateEmployeeRequest, number>('${this.appConfig.employeesUrl}', request);
    }

    update(request: UpdateEmployeeRequest): Observable<void> {
        return this.api.put<UpdateEmployeeRequest, void>(`${this.appConfig.employeesUrl}/${request.id}`, request);
    }

    delete(id: number): Observable<void> {
        return this.api.delete<void>(`${this.appConfig.employeesUrl}/${id}`);
    }
}