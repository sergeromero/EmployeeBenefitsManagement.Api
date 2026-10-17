import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { APP_CONFIG } from "@core/config/app-config.token";
import { EmployeeBenefitDto } from "@core/contracts/employee/employee-benefit.dto";
import { ApiClientService } from "../base/api-client.service";

@Injectable({
    providedIn: "root"
})
export class EmployeeEnrollmentsApiService {
    private readonly appConfig = inject(APP_CONFIG);
    private readonly api = inject(ApiClientService);

    getMyEnrollments(): Observable<EmployeeBenefitDto[]> {
        return this.api.get<EmployeeBenefitDto[]>(this.appConfig.myEnrollmentsUrl);
    }
}