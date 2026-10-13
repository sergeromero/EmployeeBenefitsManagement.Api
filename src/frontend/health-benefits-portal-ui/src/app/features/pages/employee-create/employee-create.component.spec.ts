import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { Mock } from 'vitest';
import { EmployeeCreateComponent } from './employee-create.component';
import { EmployeesApiService } from '@core/api/employees/employees-api.service';
import { AuthService } from '@core/auth/application/auth.service';
import { RoleDto } from '@core/contracts/role/role.dto';
import { EMPTY, of } from 'rxjs';
import { CreateEmployeeWithUserRequest } from '@core/contracts/employee/create-employee-with-user.request';
import { CreateEmployeeRequest } from '@core/contracts/employee/create-employee.request';
import { EmployeeFormComponent } from '../../../shared/components/employee-form/employee-form.component';
import { By } from '@angular/platform-browser';

describe('EmployeeCreateComponent', () => {
  let component: EmployeeCreateComponent;
  let fixture: ComponentFixture<EmployeeCreateComponent>;

  type GetRolesMethod = EmployeesApiService["getRoles"];
  type CreateMethod = EmployeesApiService["create"];
  type CreateWithUserMethod = EmployeesApiService["createWithUser"];

  let getRoles: Mock<GetRolesMethod>;
  let create: Mock<CreateMethod>;
  let createWithUser: Mock<CreateWithUserMethod>;

  type HasRoleMethod = AuthService["hasRole"];
  let hasRole: Mock<HasRoleMethod>;
  const isAdministrator = (role: string): boolean => role === "Administrator";

  type NavigateMethod = Router["navigate"];
  let navigate: Mock<NavigateMethod>;

  beforeEach(async () => {
    getRoles = vi.fn().mockReturnValue(of([] as RoleDto[]));
    create = vi.fn();
    createWithUser = vi.fn();
    hasRole = vi.fn().mockImplementation(isAdministrator);
    navigate = vi.fn();

    await TestBed.configureTestingModule({
        imports: [EmployeeCreateComponent],
        providers: [
            {
                provide: EmployeesApiService,
                useValue: {
                    getRoles,
                    create,
                    createWithUser
                }
            },
            {
                provide: AuthService,
                useValue: {
                    hasRole
                }
            },
            {
                provide: Router,
                useValue: {
                    navigate
                }
            }
        ]
    }).compileComponents();
  });

  function createComponent(): void {
    fixture = TestBed.createComponent(EmployeeCreateComponent);
    component = fixture.componentInstance;
  }

  function getValidRequestWithUser(): CreateEmployeeWithUserRequest {
    return {
        employee: getValidRequestEmployeeOnly(),
        user: {
            email: "anyemail@email.com",
            password: "pwd1234",
            userName: "someUser"
        },
        role: "HR"
    };
  }

  function getValidRequestEmployeeOnly(): CreateEmployeeRequest {
    return {
        employeeNumber: "EMP00001",
        firstName: "first",
        lastName: "last",
        email: "anyemail@email.com",
        hireDate: "2026-01-01",
        departmentId: 5        
    };
  }

  it('should create', () => {
    createComponent();
    expect(component).toBeTruthy();
  });

  it("should retrieve and store the available roles during initialization", () => {
    const expectedRoles: RoleDto[] = [
        { id: "1", name: "CEO" },
        { id: "2", name: "VP" },
        { id: "3", name: "Manager" },
        { id: "4", name: "Employee" }
    ];

    getRoles.mockReturnValue(of(expectedRoles));

    createComponent();

    expect(getRoles).toHaveBeenCalledOnce();
    expect(component.roles()).toEqual(expectedRoles);
  });

  it("should create an employee with an associated user when the current user is an administrator", () => {
    const request: CreateEmployeeWithUserRequest = getValidRequestWithUser();

    createWithUser.mockReturnValue(EMPTY);

    createComponent();

    const form = {
        getRawValue: vi.fn().mockReturnValue(request)
    };

    component.submit(form);

    expect(createWithUser).toHaveBeenCalledWith(request);
    expect(createWithUser).toHaveBeenCalledOnce();
    expect(create).not.toHaveBeenCalled();
  });

  it("should navigate to the employee list after successful administrator creation", () => {
    const request = getValidRequestWithUser();

    createWithUser.mockReturnValue(of(0));

    createComponent();

    const form = {
        getRawValue: vi.fn().mockReturnValue(request)
    };

    component.submit(form);

    expect(createWithUser).toHaveBeenCalledOnce();
    expect(navigate).toHaveBeenCalledOnce();
    expect(navigate).toHaveBeenCalledWith(["/employees"]);
  });

  it("should create an employee without an associated user when the current user is not an administrator", () => {
    const request = getValidRequestEmployeeOnly();

    hasRole.mockReturnValue(false);
    create.mockReturnValue(EMPTY);

    createComponent();

    const form = {
        getRawValue: vi.fn().mockReturnValue({
            employee: request,
            user: {
                email: "",
                password: "",
                suerName: ""
            },
            role: ""
        })
    };

    component.submit(form);

    expect(create).toHaveBeenCalledOnce();
    expect(create).toHaveBeenCalledWith(request);
    expect(createWithUser).not.toHaveBeenCalled();
  });

  it("should navigate to the employee list after successful non-adminmistrator creation", () => {
    const request = getValidRequestEmployeeOnly();

    hasRole.mockReturnValue(false);
    create.mockReturnValue(of(0));

    createComponent();

    const form = 
    { 
        getRawValue: vi.fn().mockReturnValue({employee: request})
    };

    component.submit(form);

    expect(create).toHaveBeenCalledOnce();
    expect(create).toHaveBeenCalledWith(request);
    expect(navigate).toHaveBeenCalledWith(["/employees"]);
  });

  it("should handle the employee form submission event", () => {
    const request = getValidRequestEmployeeOnly();

    hasRole.mockReturnValue(false);
    create.mockReturnValue(EMPTY);

    createComponent();
    fixture.detectChanges();

    const employeeFormDebugElement = fixture.debugElement.query(
        By.directive(EmployeeFormComponent)
    );

    const employeeForm = employeeFormDebugElement.componentInstance;

    const form = { getRawValue: vi.fn().mockReturnValue({
        employee: request
    })};

    employeeForm.formSubmit.emit(form);

    expect(create).toHaveBeenCalledOnce();
    expect(create).toHaveBeenCalledWith(request);
    expect(createWithUser).not.toHaveBeenCalled();
  });
});
