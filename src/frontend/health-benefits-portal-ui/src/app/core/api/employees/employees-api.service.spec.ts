import { TestBed } from "@angular/core/testing";
import { Observable, of } from "rxjs";

import { EmployeesApiService } from "./employees-api.service";
import { APP_CONFIG } from "@core/config/app-config.token";
import { ApiClientService } from "../base/api-client.service";
import { EmployeeDto } from "@core/contracts/employee/employee.dto";
import { EmployeeWithUserDto } from "@core/contracts/employee/employee-with-user.dto";
import { CreateEmployeeRequest } from "@core/contracts/employee/create-employee.request";
import { CreateEmployeeWithUserRequest } from "@core/contracts/employee/create-employee-with-user.request";
import { UpdateEmployeeRequest } from "@core/contracts/employee/update-employee.request";
import { UpdateEmployeeCompleteRequest } from "@core/contracts/employee/update-employee-complete.request";

describe("EmployeesApiService", () => {
    let service: EmployeesApiService;
    let apiGet: ReturnType<typeof vi.fn>;
    let apiPost: ReturnType<typeof vi.fn>;
    let apiPut: ReturnType<typeof vi.fn>;
    let apiDelete: ReturnType<typeof vi.fn>;
    const anyEmployeesUrl = "employees";
    const anyRolesUrl = "roles";

    beforeEach(() => {
        apiGet = vi.fn();
        apiPost = vi.fn();
        apiPut = vi.fn();
        apiDelete = vi.fn();

        TestBed.configureTestingModule({
            providers: [
                EmployeesApiService,
                {
                    provide: ApiClientService,
                    useValue: {
                        get: apiGet,
                        post: apiPost,
                        put: apiPut,
                        delete: apiDelete
                    }
                },
                {
                    provide: APP_CONFIG,
                    useValue: {
                        employeesUrl: anyEmployeesUrl,
                        rolesUrl: anyRolesUrl
                    }
                }
            ]
        });

        service = TestBed.inject(EmployeesApiService);
    });

    it("should retrieve an employee by Id", () => {
        const anyEmployeeId = 71;
        const expectedResponse$ = of({
            id: anyEmployeeId
        } as EmployeeDto);
        const expectedUrl = `${anyEmployeesUrl}/${anyEmployeeId}`;

        apiGet.mockReturnValue(expectedResponse$);

        const response$ = service.getById(anyEmployeeId);

        expect(apiGet).toHaveBeenCalledWith(expectedUrl);
        expect(response$).toBe(expectedResponse$);
    });

    it("should get an employee with its user by Id", () => {
        const anyEmployeeId = 71;
        const expectedUrl = `${anyEmployeesUrl}/${anyEmployeeId}/complete`;
        const expectedResponse$ = of({
            employee: {
                id: anyEmployeeId,
                firstName: "anyFirstName"
            } as EmployeeDto,
            user: {
                userName: "anyUserName",
                email: "anyEmail",
            }
        } as EmployeeWithUserDto);

        apiGet.mockReturnValue(expectedResponse$);

        const response$ = service.getCompleteById(anyEmployeeId);

        expect(apiGet).toHaveBeenCalledWith(expectedUrl);
        expect(response$).toBe(expectedResponse$);
    });

    it("should get all Employees using the supplied pagination and sorting parameters", () => {
        const expectedUrl = anyEmployeesUrl;
        const expectedResponse$ = of({
            items: [{
                id: 1,
                employeeNumber: "someNumber"
            }],
            totalCount: 5,
            page: 15,
            pageSize: 30
        });

        const params = {
            page: 15,
            pageSize: 30,
            sortBy: "anyKey",
            sortDirection: "anyDirection"
        };

        apiGet.mockReturnValue(expectedResponse$);

        const response$ = service.getAll(params);

        expect(apiGet).toHaveBeenCalledWith(expectedUrl, {params});
        expect(response$).toBe(expectedResponse$);
    });

    it("should create a new employee", () => {
        const request: CreateEmployeeRequest = {
            employeeNumber: "anyEmployeeNumber",
            firstName: "anyName",
            lastName: "anyLastName",
            email: "anyEmail",
            hireDate: "2026-01-30:00:00:00Z",
            departmentId: 5
        };
        const expectedUrl = anyEmployeesUrl;
        const anyNumber = 97;
        const expectedResponse$ = of(anyNumber);

        apiPost.mockReturnValue(expectedResponse$);

        const response$ = service.create(request);

        expect(apiPost).toHaveBeenCalledWith(expectedUrl, request);
        expect(response$).toBe(expectedResponse$);
    });

    it("should create a new employee with its associated user", () => {
        const request: CreateEmployeeWithUserRequest = {
            employee: {
                employeeNumber: "anyEployeeNumber",
                firstName: "anyName",
                lastName: "anyLastName",
                email: "anyEmail",
                hireDate: "2026-01-30:00:00:00Z",
                departmentId: 5                
            }, 
            user: {
                userName: "anyUserName",
                email: "anyEmail",
                password: "anyPassword"
            },
            role: "anyRole"
        };

        const expectedUrl = `${anyEmployeesUrl}/complete`;
        const anyNumber = 97;
        const expectedResponse$ = of(anyNumber);

        apiPost.mockReturnValue(expectedResponse$);

        const response$ = service.createWithUser(request);

        expect(apiPost).toHaveBeenCalledWith(expectedUrl, request);
        expect(response$).toBe(expectedResponse$);
    });

    it("should update an existing employee", () => {
        const request: UpdateEmployeeRequest = {
            id: 98,
            employeeNumber: "anyEmployeeNumber",
            firstName: "name",
            lastName: "last name",
            email: "anyEmail",
            hireDate: "2026-02-02:00:00:00Z",
            departmentId: 5
        };

        const expectedUrl = `${anyEmployeesUrl}/${request.id}`;
        const expectedResponse$: Observable<void> = of();

        apiPut.mockReturnValue(expectedResponse$);

        const response$ = service.update(request);

        expect(apiPut).toHaveBeenCalledWith(expectedUrl, request);
        expect(response$).toBe(expectedResponse$);
    });

    it("should update an existing employee with its associated user", () => {
        const request: UpdateEmployeeCompleteRequest = {
            id: 73,
            employee: {
                id: 73,
                employeeNumber: "anyEmployeeNumber",
                firstName: "name",
                lastName: "last name",
                email: "anyEmail",
                hireDate: "2026-02-02:00:00:00Z",
                departmentId: 5
            }, 
            user: {
                userName: "anyUserName",
                email: "anyEmail",
                password: ""
            },
            role: "anyRole"
        };

        const expectedUrl = `${anyEmployeesUrl}/${request.id}/complete`;
        const expectedResponse$: Observable<void> = of();

        apiPut.mockReturnValue(expectedResponse$);

        const response$ = service.updateWithUser(request);

        expect(apiPut).toHaveBeenCalledWith(expectedUrl, request);
        expect(response$).toBe(expectedResponse$);
    });

    it("should delete an employee", () => {
        const anyEmployeeId = 41;
        const expectedUrl = `${anyEmployeesUrl}/${anyEmployeeId}`;
        const expectedResponse$: Observable<void> = of();

        apiDelete.mockReturnValue(expectedResponse$);

        const response$ = service.delete(anyEmployeeId);

        expect(apiDelete).toHaveBeenCalledWith(expectedUrl);
        expect(response$).toBe(expectedResponse$);
    });

    it("should return all roles", () => {
        const expectedUrl = anyRolesUrl;
        const expectedResponse$ = of([
            {
                id: 1,
                name: "Administrator"
            }
        ]);

        apiGet.mockReturnValue(expectedResponse$);

        const response$ = service.getRoles();

        expect(apiGet).toHaveBeenCalledExactlyOnceWith(expectedUrl);
        expect(response$).toBe(expectedResponse$);
    });
});