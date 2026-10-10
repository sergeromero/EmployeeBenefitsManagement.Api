import { TestBed } from "@angular/core/testing";
import { Router } from "@angular/router";
import { AuthService } from "./auth.service";
import { TokenStorageService } from "../infrastructure/token-storage.service";
import { LoginResponse } from "@core/contracts/auth/login.response";

describe("AuthService", () => {
    let service: AuthService;
    let storageSet: ReturnType<typeof vi.fn>;
    let storageClear: ReturnType<typeof vi.fn>;
    let storageGet: ReturnType<typeof vi.fn>;
    let routerNavigate: ReturnType<typeof vi.fn>;

    beforeEach(() => {
        storageSet = vi.fn();
        storageClear = vi.fn();
        storageGet = vi.fn().mockReturnValue(null);
        routerNavigate = vi.fn();
        
        TestBed.configureTestingModule({
            providers: [
                AuthService,
                {
                    provide: TokenStorageService,
                    useValue: {
                        get: storageGet,
                        set: storageSet,
                        clear: storageClear
                    }
                },
                {
                    provide: Router,
                    useValue: {
                        navigate: routerNavigate
                    }
                }
            ]
        });
    });

    it("should start unauthenticated when no user is stored", () => {
        service = TestBed.inject(AuthService);

        expect(service.isAuthenticated()).toBe(false);
        expect(service.user()).toBeNull();
        expect(service.token()).toBeNull();
        expect(service.roles()).toEqual([]);
    });

    it("should create an authenticated session and persist it when requested", () => {
        service = TestBed.inject(AuthService);

        const loginResponse: LoginResponse = {
            accessToken: "test-token",
            expiresAt: "2099-01-01T00:00:00.000Z",
            user: {
                id: "user-123",
                email: "some@something.com",
                roles: ["Employee"],
                firstName: "FirstName",
                lastName: "LastName"
            }
        };

        service.setSession(loginResponse, true);

        expect(service.isAuthenticated()).toBe(true);
        expect(service.user()).toEqual({
            id: "user-123",
            email: "some@something.com",
            roles: ["Employee"],
            accessToken: "test-token",
            expiresAt: new Date(loginResponse.expiresAt).getTime(),
            firstName: "FirstName",
            lastName: "LastName"
        });

        expect(service.token()).toBe("test-token");
        expect(service.roles()).toEqual(["Employee"]);
        expect(storageSet).toHaveBeenCalledWith(service.user());
    });

    it("should create an authenticated session without persisting it when requested", () => {
        service = TestBed.inject(AuthService);
        
        const loginResponse: LoginResponse = {
            accessToken: "test-token",
            expiresAt: "2099-01-01T00:00:00.000Z",
            user: {
                id: "user-123",
                email: "some@something.com",
                roles: ["Employee"],
                firstName: "FirstName",
                lastName: "LastName"
            }
        };

        service.setSession(loginResponse);

        expect(service.isAuthenticated()).toBe(true);
        expect(service.token()).toBe('test-token');
        expect(service.roles()).toEqual(['Employee']);

        expect(storageSet).not.toHaveBeenCalled();
    });

    it("should clear the current session and persistent storage", () => {
        service = TestBed.inject(AuthService);
        
        const loginResponse: LoginResponse = {
            accessToken: "test-token",
            expiresAt: "2099-01-01T00:00:00.000Z",
            user: {
                id: "user-123",
                email: "some@something.com",
                roles: ["Employee"],
                firstName: "FirstName",
                lastName: "LastName"
            }
        };

        service.setSession(loginResponse, true);

        service.clear();

        expect(service.isAuthenticated()).toBe(false);
        expect(service.user()).toBeNull();
        expect(service.token()).toBeNull();
        expect(service.roles()).toEqual([]);
        expect(storageClear).toHaveBeenCalled();
    });

    it("should restore a valid session from storage", () => {
        const storedUser = {
            id: "user-123",
            email: "some@something.com",
            roles: ["Employee"],
            accessToken: "stored-token",
            expiresAt: new Date("2099-01-01T00:00:00.000Z").getTime(),
            firstName: "FirstName",
            lastName: "LastName"
        };

        storageGet.mockReturnValue(storedUser);

        service = TestBed.inject(AuthService);

        expect(service.isAuthenticated()).toBe(true);
        expect(service.user()).toEqual(storedUser);
        expect(service.token()).toBe("stored-token");
        expect(service.roles()).toEqual(["Employee"]);

        expect(storageGet).toHaveBeenCalled();
    });

    it("should clear an expired stored session", () => {
        const expiredUser = {
            id: "user-123",
            email: "some@something.com",
            roles: ["Employee"],
            accessToken: "expired-token",
            expiresAt: new Date("2000-01-01T00:00:00.000Z").getTime(),
            firstName: "FirstName",
            lastName: "LastName"
        };

        storageGet.mockReturnValue(expiredUser);

        service = TestBed.inject(AuthService);

        expect(service.isAuthenticated()).toBe(false);
        expect(service.user()).toBeNull();
        expect(service.token()).toBeNull();
        expect(service.roles()).toEqual([]);

        expect(storageClear).toHaveBeenCalled();
    });

    it("should return true when the user has the specified role", () => {
        service = TestBed.inject(AuthService);

        const loginResponse: LoginResponse = {
            accessToken: "test-token",
            expiresAt: "2099-01-01T00:00:00.000Z",
            user: {
                id: "user-123",
                email: "some@something.com",
                roles: ["Employee", "HR"],
                firstName: "FirstName",
                lastName: "LastName" 
            }           
        };

        service.setSession(loginResponse);

        expect(service.hasRole("HR")).toBe(true);
    });

    it("should return false when the user does not have the specified role", () => {
        service = TestBed.inject(AuthService);

        const loginResponse: LoginResponse = {
            accessToken: "test-token",
            expiresAt: "2099-01-01T00:00:00.000Z",
            user: {
                id: "user-123",
                email: "some@something.com",
                roles: ["Employee", "HR"],
                firstName: "FirstName",
                lastName: "LastName"
            }
        };

        service.setSession(loginResponse);

        expect(service.hasRole("Administrator")).toBe(false);
    });

    it("should clear the session and navigate to the login page", () => {
        service = TestBed.inject(AuthService);

        const loginResponse: LoginResponse = {
            accessToken: "test-token",
            expiresAt: "2099-01-01T00:00:00.000Z",
            user: {
                id: "user-123",
                email: "some@something.com",
                roles: ["Employee"],
                firstName: "FirstName",
                lastName: "LastName"
            }
        };

        service.setSession(loginResponse, true);

        service.logout();

        expect(service.isAuthenticated()).toBe(false);
        expect(service.user()).toBeNull();

        expect(storageClear).toHaveBeenCalled();
        expect(routerNavigate).toHaveBeenCalledWith(["/login"]);
    });
});