import { ComponentFixture, TestBed } from "@angular/core/testing";
import { of } from "rxjs";

import { AuthApiService } from "./auth-api.service";
import { ApiClientService } from "../base/api-client.service";
import { APP_CONFIG } from "@core/config/app-config.token";
import { LoginRequest } from "@core/contracts/auth/login.request";

describe("AuthApiService", () => {
    let service: AuthApiService;
    let apiPost: ReturnType<typeof vi.fn>;

    beforeEach(() => {
        apiPost = vi.fn();

        TestBed.configureTestingModule({
            providers: [
                AuthApiService,
                {
                    provide: ApiClientService,
                    useValue: {
                        post: apiPost
                    }
                },
                {
                    provide: APP_CONFIG,
                    useValue: {
                        loginUrl: "auth/login"
                    }
                }
            ]
        });

        service = TestBed.inject(AuthApiService);
    });

    it("should delegate login request to the API client", () => {
        const loginRequest: LoginRequest = {
            email: "something@domain.com",
            password: "somePassword"
        };

        apiPost.mockReturnValue(of());

        service.login(loginRequest);

        expect(apiPost).toHaveBeenCalledWith("auth/login", loginRequest);
    });

    it("should return the Observable provided by the API client", () => {
        const loginRequest: LoginRequest = {
            email: "anyemail@domain.com",
            password: "anypassword"
        };

        const expectedResponse$ = of({
            accessToken: "any-token",
            expiresAt: "2027-01-01T00:00:00Z",
            user: {
                id: "anyId",
                email: "anyemail@domain.com",
                roles: ["Employee"],
                firstName: "Any First Name",
                lastName: "Any Last Name"
            }
        });

        apiPost.mockReturnValue(expectedResponse$);

        const result$ = service.login(loginRequest);

        expect(result$).toBe(expectedResponse$);
    });
});