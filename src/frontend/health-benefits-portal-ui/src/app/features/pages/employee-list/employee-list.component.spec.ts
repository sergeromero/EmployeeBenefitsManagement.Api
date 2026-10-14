import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';

import { EmployeeListComponent } from './employee-list.component';
import { EmployeesApiService } from '@core/api/employees/employees-api.service';
import { AuthService } from '@core/auth/application/auth.service';
import { Mock } from 'vitest';
import { PagedResult } from '@core/contracts/models/paged-result.model';
import { EmployeeListItem } from '@core/contracts/models/employee-list-item.model';

describe('EmployeeListComponent', () => {
    let component: EmployeeListComponent;
    let fixture: ComponentFixture<EmployeeListComponent>;

    let getAll: Mock<EmployeesApiService["getAll"]>;
    let hasRole: Mock<AuthService["hasRole"]>;
    let navigate: Mock<Router["navigate"]>;
    const isAdministrator = (role: string): boolean => role === "Administrator";
        
    beforeEach(async () => {
        getAll = vi.fn<EmployeesApiService["getAll"]>();
        getAll.mockReturnValue(of({
            items: [],
            totalCount: 0,
            page: 1,
            pageSize: 10
        }));

        hasRole = vi.fn<AuthService["hasRole"]>();
        hasRole.mockReturnValue(isAdministrator(""));

        navigate = vi.fn<Router["navigate"]>();

        await TestBed.configureTestingModule({
            imports: [EmployeeListComponent],
            providers: [
                {
                    provide: EmployeesApiService,
                    useValue: { getAll }
                },
                {
                    provide: AuthService,
                    useValue: { hasRole }
                },
                {
                    provide: Router,
                    useValue: { navigate }
                }
            ]
        }).compileComponents();
    });

    function createComponent(): void {
        fixture = TestBed.createComponent(EmployeeListComponent);
        component = fixture.componentInstance;
    }

    function getListOfEmployees(): PagedResult<EmployeeListItem> {
        return {
            items: [
                {
                    id: 42,
                    employeeNumber: "EMP00001",
                    firstName: "first",
                    lastName: "last",
                    email: "anyemail@email.com",
                    hireDate: "2025-01-01",
                    departmentName: "Engineering",
                    userName: "jane.doe",
                    hasUser: true
                },
                {
                    id: 63,
                    employeeNumber: "EMP00002",
                    firstName: "other",
                    lastName: "name",
                    email: "someemail@email.com",
                    hireDate: "2026-06-06",
                    departmentName: "Sales",
                    userName: "",
                    hasUser: false
                }
            ],
            totalCount: 2,
            page: 1,
            pageSize: 10
        };
    }

    it('should create', () => {
        createComponent();
        expect(component).toBeTruthy();
    });

    it("should request the initial employee list with the expected parameters", () => {
        createComponent();

        expect(getAll).toHaveBeenCalledExactlyOnceWith({
            page: 1,
            pageSize: 10,
            sortBy: "lastName",
            sortDirection: "asc"
        });
    });

    it("should update the employee list and total count when the API responds", () => {
        const response = getListOfEmployees();

        getAll.mockReturnValue(of(response));

        createComponent();

        expect(component.employees()).toEqual(response.items);
        expect(component.totalCount()).toBe(2);
    });

    it("should display the appropriate columns for an administrator", () => {
        hasRole.mockImplementation(role => {
            if(role === "Administrator") {
                return true;
            } else if (role === "HR") {
                return false;
            }

            return false;
        });

        createComponent();

        expect(component.displayedColumns).toEqual([
            "employeeNumber",
            "firstName",
            "lastName",
            "email",
            "hireDate",
            "departmentName",
            "userName",
            "actions"
        ]);
    });

    it("should display the appropriate columns for HR", () => {
        hasRole.mockImplementation(role => {
            if(role === "Administrator") {
                return false;
            } else if (role === "HR") {
                return true;
            }

            return false;
        });

        createComponent();

        expect(component.displayedColumns).toEqual([
            "employeeNumber",
            "firstName",
            "lastName",
            "email",
            "hireDate",
            "departmentName",
            "hasUser",
            "actions"
        ]);
    });

    it("should display the appropriate columns for a regular employee", () => {
        hasRole.mockImplementation(role => {
            if(role === "Administrator") {
                return false;
            } else if (role === "HR") {
                return false;
            }

            return false;
        });

        createComponent();

        expect(component.displayedColumns).toEqual([
            "employeeNumber",
            "firstName",
            "lastName",
            "email",
            "hireDate",
            "departmentName",
            "hasUser"
        ]);        
    });

    it("should update pagination and reload employees when the page changes", () => {
        createComponent();

        const pageEvent = {
            pageIndex: 2, 
            pageSize: 15,
            length: 25
        };

        getAll.mockClear();

        component.onPageChange(pageEvent);

        expect(component.page()).toBe(3);
        expect(component.pageSize()).toBe(15);

        expect(getAll).toHaveBeenCalledExactlyOnceWith({
            page: 3,
            pageSize: 15,
            sortBy: "lastName",
            sortDirection: "asc"
        });
    });

    it("should update sorting and reload employees when the sort changes", () => {
        createComponent();

        const sortEvent = {
            active: "email",
            direction: "desc"
        };

        getAll.mockClear();

        component.onSortChange(sortEvent);

        expect(component.sortBy()).toBe("email");
        expect(component.sortDirection()).toBe("desc");

        expect(getAll).toHaveBeenCalledExactlyOnceWith({
            page: 1,
            pageSize: 10,
            sortBy: "email",
            sortDirection: "desc"
        });
    });

    it("should default to ascending order when the sort direction is empty", () => {
        createComponent();

        const sortEvent = {
            active: "firstName",
            direction: ""
        };

        getAll.mockClear();

        component.onSortChange(sortEvent);

        expect(component.sortBy()).toBe("firstName");
        expect(component.sortDirection()).toBe("asc");

        expect(getAll).toHaveBeenCalledExactlyOnceWith({
            page: 1,
            pageSize: 10,
            sortBy: "firstName",
            sortDirection: "asc"
        });
    });

    it("should navigate to the selected employee edit page", () => {
        const anyId = 386;
        createComponent();

        const employee: EmployeeListItem = {
            id: anyId,
            employeeNumber: "EMP00823",
            firstName: "Jane",
            lastName: "Doe",
            email: "any-email@email.com",
            hireDate: "2023-08-15",
            departmentName: "Finance",
            userName: "jane.doe",
            hasUser: true
        };

        component.editEmployee(employee);

        expect(navigate).toHaveBeenCalledExactlyOnceWith([
            "/employees",
            anyId,
            "edit"
        ]);
    });

    it("should render employee records returned by the API", () => {
        const response: PagedResult<EmployeeListItem> = getListOfEmployees();

        getAll.mockReturnValue(of(response));

        createComponent();

        fixture.detectChanges();

        const nativeElement: HTMLElement = fixture.nativeElement;

        const rows = nativeElement.querySelectorAll("tr[mat-row]");

        expect(rows).toHaveLength(2);

        assertColumnContent(nativeElement, "td.mat-column-employeeNumber", "EMP00001", "EMP00002");
        assertColumnContent(nativeElement, "td.mat-column-firstName", "first", "other");
        assertColumnContent(nativeElement, "td.mat-column-lastName", "last", "name");
        assertColumnContent(nativeElement, "td.mat-column-email", "anyemail@email.com", "someemail@email.com");
        assertColumnContent(nativeElement, "td.mat-column-hireDate", "2025-01-01", "2026-06-06");
        assertColumnContent(nativeElement, "td.mat-column-departmentName", "Engineering", "Sales");
    });

    function assertColumnContent(nativeElement: HTMLElement, selector: string, firstRecordContent: string, secondRecordContent: string): void {
        const records = nativeElement.querySelectorAll(selector);
        expect(records?.item(0).textContent).toBe(firstRecordContent);
        expect(records?.item(1).textContent).toBe(secondRecordContent);
    }

    it("should render the appropriate columns for an administrator", () => {
        hasRole.mockImplementation(role => role === "Administrator");

        createComponent();

        fixture.detectChanges();

        const nativeElement: HTMLElement = fixture.nativeElement;

        const headers = Array.from(
            nativeElement.querySelectorAll("th[mat-header-cell]")
        ).map(header => header.textContent?.trim());

        expect(headers).toEqual([
            "Employee #",
            "First Name",
            "Last Name",
            "Email",
            "Hire Date",
            "Department Name",
            "User Name",
            "Actions"
        ]);
        
        expect(nativeElement.querySelector("th.mat-column-userName")).not.toBeNull(); 
        expect(nativeElement.querySelector("th.mat-column-hasUser")).toBeNull();
    });

    it("should render the appropriate columns for an HR user", () => {
        hasRole.mockImplementation(role => role === "HR");

        createComponent();

        fixture.detectChanges();

        const nativeElement: HTMLElement = fixture.nativeElement;

        const headers = Array.from(
            nativeElement.querySelectorAll("th[mat-header-cell]")
        ).map(header => header.textContent?.trim());

        expect(headers).toEqual([
            "Employee #",
            "First Name",
            "Last Name",
            "Email",
            "Hire Date",
            "Department Name",
            "Has User",
            "Actions"
        ]);

        expect(nativeElement.querySelector("th.mat-column-userName")).toBeNull();
        expect(nativeElement.querySelector("th.mat-column-hasUser")).not.toBeNull();
    });

    it("should render the appropriate columns for a regular employee", () => {
        hasRole.mockReturnValue(false);

        createComponent();

        fixture.detectChanges();

        const nativeElement: HTMLElement = fixture.nativeElement;

        const headers = Array.from(
            nativeElement.querySelectorAll("th[mat-header-cell]")
        ).map(header => header.textContent?.trim());

        expect(headers).toEqual([
            "Employee #",
            "First Name",
            "Last Name",
            "Email",
            "Hire Date",
            "Department Name",
            "Has User"
        ]);

        expect(nativeElement.querySelector("th.mat-column-hasUser")).not.toBeNull();
        expect(nativeElement.querySelector("th.mat-column-userName")).toBeNull();
        expect(nativeElement.querySelector("th.mat-column-actions")).toBeNull();        
    });

    it("should navigate to the selected employee edit page when the edit button is clicked", () => {
        hasRole.mockImplementation(role => role === "Administrator");

        const response = getListOfEmployees();

        getAll.mockReturnValue(of(response));

        createComponent();
        fixture.detectChanges();

        const nativeElement: HTMLElement = fixture.nativeElement;
        const editButtons = nativeElement.querySelectorAll<HTMLButtonElement>(
            "button[mat-icon-button"
        );

        expect(editButtons.length).toBe(2);

        editButtons.item(0).click();

        expect(navigate).toHaveBeenCalledExactlyOnceWith([
            "/employees",
            42,
            "edit"
        ]);
    });
});