import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import { ApiClientService } from "../base/api-client.service";
import { LoginRequest } from "../../contracts/auth/login.request";
import { LoginResponse } from "../../contracts/auth/login.response";
import { APP_CONFIG } from "@core/config/app-config.token";

@Injectable({
    providedIn: "root"
})
export class AuthApiService {
    private readonly appConfig = inject(APP_CONFIG);
    private api = inject(ApiClientService);

    login(request: LoginRequest): Observable<LoginResponse> {
        return this.api.post<LoginRequest, LoginResponse>(this.appConfig.loginUrl, request);
    }
}