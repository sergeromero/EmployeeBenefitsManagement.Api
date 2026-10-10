import { TestBed } from "@angular/core/testing";
import { provideHttpClient } from "@angular/common/http";
import { provideHttpClientTesting } from "@angular/common/http/testing";
import { HttpTestingController } from "@angular/common/http/testing";

import { ApiClientService } from "./api-client.service";
import { APP_CONFIG } from "@core/config/app-config.token";

describe("ApiClientService", () => {
    let service: ApiClientService;
    let httpTestingController: HttpTestingController;

    const anyUrl = "https://localhost:1234/api";

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                ApiClientService,
                provideHttpClient(),
                provideHttpClientTesting(),
                {
                    provide: APP_CONFIG,
                    useValue: {
                        apiBaseUrl: anyUrl
                    }
                }
            ]
        });

        service = TestBed.inject(ApiClientService);
        httpTestingController = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpTestingController.verify();
    })

    it("should send a GET request to the configured API endpoint", () => {
        const anyEndpoint = "anyendpoint";
        const expectedUrl = `${anyUrl}/${anyEndpoint}`;

        service.get(anyEndpoint).subscribe();

        const request = httpTestingController.expectOne(expectedUrl);

        expect(request.request.method).toBe("GET");
    });

    it("should return the response received from the server", () => {
        const anyEndpoint = "anyendpoint";
        const expectedUrl = `${anyUrl}/${anyEndpoint}`;

        const expectedEmployees = [
            { id: 1, firstName: "First", lastName: "Last" }
        ];

        let actualEmployees: typeof expectedEmployees | undefined;

        service.get<typeof expectedEmployees>(anyEndpoint).subscribe(result => {
            actualEmployees = result;
        });

        const request = httpTestingController.expectOne(expectedUrl);
        request.flush(expectedEmployees);
        expect(actualEmployees).toEqual(expectedEmployees);
    });

    it("should include query parameters in the GET request", () => {
        const anyEndpoint = "anyendpoint";
        const expectedUrl = `${anyUrl}/${anyEndpoint}`;

        const params = {
            page: 2,
            pageSize: 10
        };

        service.get(anyEndpoint, { params }).subscribe();

        const request = httpTestingController.expectOne(
            req => req.url === expectedUrl
        );

        expect(request.request.params.get("page")).toBe("2");
        expect(request.request.params.get("pageSize")).toBe("10");
    });

    it("should exclude null and undefined values of parameters in the GET request", () => {
        const anyEndpoint = "anyendpoint";
        const expectedUrl = `${anyUrl}/${anyEndpoint}`;

        const params = {
            page: 2,
            pageSize: 10,
            departmentId: null,
            otherValue: undefined
        };

        service.get(anyEndpoint, { params }).subscribe();

        const request = httpTestingController.expectOne(
            req => req.url === expectedUrl
        );

        expect(request.request.params.get("page")).toBe("2");
        expect(request.request.params.get("pageSize")).toBe("10");
        expect(request.request.params.get("departmentId")).toBeNull();
        expect(request.request.params.get("otherValue")).toBeNull();
    });

    it("should send a POST request with the supplied body", () => {
        const anyEndpoint = "anyendpoint";
        const expectedUrl = `${anyUrl}/${anyEndpoint}`;

        const anyBody = {
            data: "empty",
            number: 0,
            address: "1234 some street"
        };

        service.post(anyEndpoint, anyBody).subscribe();

        const request = httpTestingController.expectOne(expectedUrl);

        expect(request.request.method).toBe("POST");
        expect(request.request.body).toEqual(anyBody);
    });

    it("should send a PUT request with the supplied body", () => {
        const anyEndpoint = "anyendpoint";
        const expectedUrl = `${anyUrl}/${anyEndpoint}`;

        const anyBody = {
            data: "empty",
            number: 0,
            address: "1234 some street"
        };

        service.put(anyEndpoint, anyBody).subscribe();

        const request = httpTestingController.expectOne(expectedUrl);

        expect(request.request.method).toBe("PUT");
        expect(request.request.body).toEqual(anyBody);
    });

    it("should send a DELETE request", () => {
        const anyEndpoint = "anyendpoint";
        const expectedUrl = `${anyUrl}/${anyEndpoint}`;

        service.delete(anyEndpoint).subscribe();

        const request = httpTestingController.expectOne(expectedUrl);

        expect(request.request.method).toBe("DELETE");
    });
});