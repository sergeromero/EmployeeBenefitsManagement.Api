import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { APP_CONFIG } from "@core/config/app-config.token";
import { BenefitPlanListItemDto } from "@core/contracts/benefit-plan/benefit-plan-list-item.dto";
import { ApiClientService } from "../base/api-client.service";

@Injectable({
    providedIn: "root"
})
export class BenefitPlansApiService {
    private readonly api = inject(ApiClientService);
    private readonly appConfig = inject(APP_CONFIG);

    getAll(): Observable<BenefitPlanListItemDto[]> {
        return this.api.get<BenefitPlanListItemDto[]>(this.appConfig.benefitPlansUrl);
    }
}