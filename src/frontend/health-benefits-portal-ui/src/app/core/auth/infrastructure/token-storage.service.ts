import { Injectable, inject, PLATFORM_ID } from "@angular/core";
import { isPlatformBrowser } from "@angular/common";
import { AUTH_STORAGE_KEY } from "../constants/auth.constants";
import { AuthenticatedUser } from "../domain/models/authenticated-user.model";

@Injectable ({
    providedIn: "root"
})
export class TokenStorageService {
    private readonly platformId = inject(PLATFORM_ID);
    private readonly isBrowser = isPlatformBrowser(this.platformId);

    get(): AuthenticatedUser | null {
        if(!this.isBrowser) {
            return null;
        }
        
        const raw = localStorage.getItem(AUTH_STORAGE_KEY);
        if (!raw) return null;

        try {
            return JSON.parse(raw) as AuthenticatedUser;
        } catch {
            this.clear();
            return null;
        }
    }

    set(session: AuthenticatedUser): void {
        if (this.isBrowser){
            localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(session));
        }        
    }

    clear(): void {
        if (this.isBrowser) {
            localStorage.removeItem(AUTH_STORAGE_KEY);
        }        
    }
}