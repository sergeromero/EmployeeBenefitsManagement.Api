import { Injectable, computed, signal } from "@angular/core";
import { TokenStorageService } from "../infrastructure/token-storage.service";
//import { decodeJwt, extractRoles } from "../domain/utils/jwt.utils";
import { AuthenticatedUser } from "../domain/models/authenticated-user.model";
import { LoginResponse } from "@core/contracts/auth/login.response";

@Injectable({
    providedIn: "root"
})
export class AuthService {
    private readonly _user = signal<AuthenticatedUser | null>(null);

    readonly user = computed(() => this._user());
    readonly token = computed(() => this._user()?.accessToken ?? null);
    readonly roles = computed(() => this._user()?.roles ?? []);

    constructor(private readonly storage: TokenStorageService) {
        this.restoreFromStorage();
    }

    readonly isAuthenticated = computed (() => {
        const user = this._user();
        if(!user) return false;

        return Date.now() < user.expiresAt;
    });

    setSession(response: LoginResponse, persist: boolean = false): void {
        const expiresAt = new Date(response.expiresAt).getTime();

        const user: AuthenticatedUser = {
            id: response.user.id,
            email: response.user.email,
            roles: response.user.roles,
            accessToken: response.accessToken,
            expiresAt
        };

        this._user.set(user);

        if (persist) {
            this.storage.set(user);
        }
    }

    clear(): void {
        this._user.set(null);
        this.storage.clear();
    }

    hasRole(role: string): boolean {
        return this.roles().includes(role);
    }

    private restoreFromStorage(): void {
        const storedUser = this.storage.get();
        if (!storedUser) return;

        if (Date.now() >= storedUser.expiresAt) {
            this.clear();
            return;
        }

        this._user.set(storedUser);
    }
}