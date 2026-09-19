import { Injectable, inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { APP_CONFIG } from "../../config/app-config.token";
import { Observable } from "rxjs";

@Injectable({
    providedIn: "root"
})
export class ApiClientService {
    private http = inject(HttpClient);
    private appConfig = inject(APP_CONFIG);

    private baseUrl = this.appConfig.apiBaseUrl;

    get<T>(endpoint: string): Observable<T> {
        return this.http.get<T>(this.buildUrl(endpoint));
    }

    post<TRequest, TResponse>(endpoint: string, body: TRequest): Observable<TResponse> {
        return this.http.post<TResponse>(this.buildUrl(endpoint), body);
    }

    put<TRequest, TResponse>(endpoint: string, body: TRequest): Observable<TResponse> {
        return this.http.put<TResponse>(this.buildUrl(endpoint), body);
    }

    delete<T>(endpoint: string): Observable<T> {
        return this.http.delete<T>(this.buildUrl(endpoint));
    }

    private buildUrl(endpoint: string): string {
        return `${this.baseUrl}/${endpoint}`;
    }
}