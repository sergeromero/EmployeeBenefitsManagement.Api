import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { APP_CONFIG } from "@core/config/app-config.token";
import { EmployeeBenefitDto } from "@core/contracts/employee/employee-benefit.dto";
import { ApiClientService } from "../base/api-client.service";
import { CreateEnrollmentRequest } from "@core/contracts/enrollment/create-enrollment.request";
import { TerminateEnrollmentRequest } from "@core/contracts/enrollment/terminate-enrollment.request";

@Injectable({
    providedIn: "root"
})
export class EmployeeEnrollmentsApiService {
    private readonly appConfig = inject(APP_CONFIG);
    private readonly api = inject(ApiClientService);

    getMyEnrollments(): Observable<EmployeeBenefitDto[]> {
        return this.api.get<EmployeeBenefitDto[]>(this.appConfig.myEnrollmentsUrl);
    }

    create(request: CreateEnrollmentRequest): Observable<void> {
        return this.api.post<CreateEnrollmentRequest, void>(this.appConfig.enrollmentsUrl, request);
    }

    terminate(request: TerminateEnrollmentRequest): Observable<void> {
        return this.api.post<TerminateEnrollmentRequest, void>(`${this.appConfig.enrollmentsUrl}/terminate`, request);
    }
}