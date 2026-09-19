import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import { ApiClientService } from "../base/api-client.service";
import { LoginRequest } from "../../contracts/auth/login.request";
import { LoginResponse } from "../../contracts/auth/login.response";

@Injectable({
    providedIn: "root"
})
export class AuthApiService {
    private api = inject(ApiClientService);

    login(request: LoginRequest): Observable<LoginResponse> {
        return this.api.post<LoginRequest, LoginResponse>("auth/login", request);
    }
}