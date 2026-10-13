import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EmployeeFormComponent } from './employee-form.component';
import { type Mock } from 'vitest';
import { AuthService } from '@core/auth/application/auth.service';

describe('EmployeeFormComponent', () => {
  let component: EmployeeFormComponent;
  let fixture: ComponentFixture<EmployeeFormComponent>;

  type HasRoleMethod = AuthService["hasRole"];

  let hasRole: Mock<HasRoleMethod>;

  const isAdministrator = (role: string): boolean => role === "Administrator";

  beforeEach(async () => {
    hasRole = vi.fn().mockImplementation(isAdministrator);

    await TestBed.configureTestingModule({
      imports: [EmployeeFormComponent],
      providers: [
        {
          provide: AuthService,
          useValue: {
            hasRole
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EmployeeFormComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it("should initialize the form with the expected default values", () => {
    expect(component.form.getRawValue()).toEqual({
      employee: {
        id: 0,
        employeeNumber: "",
        firstName: "",
        lastName: "",
        email: "",
        hireDate: "",
        departmentId : 0
      },
      user: {
        userName: "",
        email: "",
        password: "",
      },
      role: ""
    });
  });

  it.each([
    "employeeNumber",
    "firstName",
    "lastName",
    "email",
    "hireDate"
  ])("should require the employee.%s field", (field) => {
    const control = component.form.get(`employee.${field}`)

    expect(control?.hasError("required")).toBe(true);
  });

  it("should reject an invalid employee email address", () => {
    const emailControl = component.form.get("employee.email");

    emailControl?.setValue("not-a-valid-email");

    expect(emailControl?.hasError("email")).toBe(true);
  });

  it("should require username and email for administrators", () => {
    const userNameControl = component.form.get("user.userName");
    const userEmailControl = component.form.get("user.email");

    expect(userNameControl?.hasError("required")).toBe(true);
    expect(userEmailControl?.hasError("required")).toBe(true);

    expect(hasRole).toHaveBeenCalledWith("Administrator");
  });

  it("should reject an invalid user account email address", () => {
    const emailControl = component.form.get("user.email");

    emailControl?.setValue("not-a-valid-email");

    expect(emailControl?.hasError("email")).toBe(true);
  });

  it("should require a password when an administrator creates an employee", () => {
    const passwordControl = component.form.get("user.password");

    expect(passwordControl?.hasError("required")).toBe(true);
  });

  it("should not require a password when an administrator edits an employee", () => {
    const editFixture = TestBed.createComponent(EmployeeFormComponent);

    editFixture.componentInstance.mode = "edit";
    editFixture.detectChanges();

    const passwordControl = editFixture.componentInstance.form.get("user.password");

    expect(passwordControl?.hasError("required")).toBe(false);

    editFixture.destroy();
  });

  it("should disable user account and role controls for non-administrators", () => {
    hasRole.mockImplementation(() => false);

    const nonAdminFixture = TestBed.createComponent(EmployeeFormComponent);

    nonAdminFixture.detectChanges();

    const form = nonAdminFixture.componentInstance.form;

    expect(form.get("user")?.disabled).toBe(true);
    expect(form.get("role")?.disabled).toBe(true);
    expect(form.get("employee")?.enabled).toBe(true);

    nonAdminFixture.destroy();
  });

  it("should hide the user account section for non-administrators", () => {
    hasRole.mockImplementation(() => false);

    const nonAdminFixture = TestBed.createComponent(EmployeeFormComponent);

    nonAdminFixture.detectChanges();

    const compiled = nonAdminFixture.nativeElement as HTMLElement;

    expect(compiled.querySelector("h3.section-title")?.textContent).toContain("Employee Information");
    
    expect(compiled.textContent).not.toContain("User Account");
    expect(compiled.querySelector("mat-select")).toBeNull();

    nonAdminFixture.destroy();
  });

  it('should display the user account section for administrators', () => {
    const compiled = fixture.nativeElement as HTMLElement;

    expect(compiled.textContent).toContain('User Account');
    expect(compiled.querySelector('input[formControlName="userName"]')).not.toBeNull();
    expect(compiled.querySelector('mat-select')).not.toBeNull();
    expect(hasRole).toHaveBeenCalledWith("Administrator");
  });

  it("should not emit formSubmit when the form is invalid", () => {
    const emitSpy = vi.spyOn(component.formSubmit, "emit");

    component.submit();

    expect(emitSpy).not.toHaveBeenCalled();
  });

  it("should emit the form when the form is valid", () => {
    const emitSpy = vi.spyOn(component.formSubmit, "emit");

      component.form.patchValue({
        employee: {
          employeeNumber: 'EMP001',
          firstName: 'John',
          lastName: 'Smith',
          email: 'john.smith@example.com',
          hireDate: '2026-01-15'
        },
        user: {
          userName: 'john.smith',
          email: 'john.smith@example.com',
          password: 'SecurePassword123'
        },
        role: 'Employee'
      });

      expect(component.form.valid).toBe(true);

      component.submit();

      expect(emitSpy).toHaveBeenCalledExactlyOnceWith(component.form);
  });

  it.each([
    { loading: false, expectedDisabled: false },
    { loading: true, expectedDisabled: true }
  ])("should set the submit button disabled state to $expectedDisabled when loadins is $loading", ({loading, expectedDisabled}) =>  {
    component.form.patchValue({
      employee: {
        employeeNumber: 'EMP001',
        firstName: 'John',
        lastName: 'Smith',
        email: 'john.smith@example.com',
        hireDate: '2026-01-15'
      },
      user: {
        userName: 'john.smith',
        email: 'john.smith@example.com',
        password: 'SecurePassword123'
      }
    });

    component.loading.set(loading);
    fixture.detectChanges();

    const button = fixture.nativeElement.querySelector("button[type='submit']") as HTMLButtonElement;

    expect(component.form.valid).toBe(true);
    expect(button.disabled).toBe(expectedDisabled);
  });

  it("should populate the form with initial data when editing an employee", () => {
    const initialData = {
      employee: {
        id: 42,
        employeeNumber: "EMP042",
        firstName: "Jane",
        lastName: "Doe",
        email: "jane.doe@example.com",
        hireDate: "2024-05-15",
        departmentId: 3
      },
      user: {
        userName: "jane.doe",
        email: "jane.doe@example.com"
      },
      role: "HR"
    };

    const editFixture = TestBed.createComponent(EmployeeFormComponent);
    editFixture.componentInstance.mode = "edit";
    editFixture.componentInstance.initialData = initialData;

    editFixture.detectChanges();

    expect(editFixture.componentInstance.form.getRawValue()).toEqual({
      employee: {
        id: 42,
        employeeNumber: "EMP042",
        firstName: "Jane",
        lastName: "Doe",
        email: "jane.doe@example.com",
        hireDate: "2024-05-15",
        departmentId: 3
      },
      user: {
        userName: "jane.doe",
        email: "jane.doe@example.com",
        password: ""
      },
      role: "HR"
    });
  });
});
