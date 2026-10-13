import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppLogin } from './login.component';
import { AuthApiService } from '@core/api/auth/auth-api.service';
import { AuthService } from '@core/auth/application/auth.service';
import { Router } from '@angular/router';
import { Observable, of, Subject, throwError } from 'rxjs';
import { LoginResponse } from '@core/contracts/auth/login.response';
import { Mock } from 'vitest';
import { LoginRequest } from '@core/contracts/auth/login.request';

describe('AppLogin', () => {
    let component: AppLogin;
    let fixture: ComponentFixture<AppLogin>;

    type LoginMethod = AuthApiService["login"];
    type SetSessionMethod = AuthService["setSession"];
    type NavigateByUrlMethod = Router["navigateByUrl"];
    let login: Mock<LoginMethod>;
    let setSession: Mock<SetSessionMethod>;
    let navigateByUrl: Mock<NavigateByUrlMethod>;

    beforeEach(async () => {
        login = vi.fn();
        setSession = vi.fn();
        navigateByUrl = vi.fn();

        await TestBed.configureTestingModule({
            imports: [AppLogin],
            providers: [
                {
                    provide: AuthApiService,
                    useValue: {
                        login
                    }
                },
                {
                    provide: AuthService,
                    useValue: {
                        setSession
                    }
                },
                {
                    provide: Router,
                    useValue: {
                        navigateByUrl
                    }
                }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(AppLogin);
        component = fixture.componentInstance;

        await fixture.whenStable();
    });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.loading()).toBe(false);
    expect(component.errorMessage()).toBeNull();
  });

  it("should mark all controls as touched and not attempt to login when the form is invalid", () => {
    component.submit();

    expect(component.form.controls.email.touched).toBe(true);
    expect(component.form.controls.password.touched).toBe(true);
    expect(login).toHaveBeenCalledTimes(0);
  });

  it("should disable the button when the form is invalid", () => {
    const button = fixture.nativeElement.querySelector(".login-button");

    expect(button.disabled).toBe(true);
  });

  it("should log in successfully and navigate to the home page when submitting a valid form", () => {
    const anyValidEmail = "something@email.com";
    const anyValidPassword = "123456";

    component.form.controls.email.setValue(anyValidEmail);
    component.form.controls.password.setValue(anyValidPassword);

    const loginResponse: LoginResponse = {
        accessToken: "test-access-token",
        expiresAt: "2026-12-31T23:59:59Z",
        user: {
            id: "user-123",
            email: anyValidEmail,
            roles: ["Employee"],
            firstName: "Test",
            lastName: "User"
        }        
    };

    login.mockReturnValue(of(loginResponse));

    component.submit();

    expect(component.errorMessage()).toBeNull();
    expect(component.loading()).toBe(false);

    expect(login).toHaveBeenCalledWith({email: anyValidEmail, password: anyValidPassword});
    expect(setSession).toHaveBeenCalledWith(loginResponse, true);
    expect(navigateByUrl).toHaveBeenCalledWith("/");
  });
  
  it("should display an invalid credentials message when login fails with HTTP 401", ()=> {
    const anyValidEmail = "something@email.com";
    const anyValidPassword = "123456";

    component.form.controls.email.setValue(anyValidEmail);
    component.form.controls.password.setValue(anyValidPassword);

    login.mockReturnValue(throwError(() => ({ status: 401})));

    component.submit();

    expect(login).toHaveBeenCalledWith({email: anyValidEmail, password: anyValidPassword});
    expect(setSession).toHaveBeenCalledTimes(0);
    expect(navigateByUrl).toHaveBeenCalledTimes(0);
    expect(component.loading()).toBe(false);

    expect(component.errorMessage()).toBe("Invalid email or password.");
  });

  it("should display the error returned by the API", () => {
    const anyValidEmail = "something@email.com";
    const anyValidPassword = "123456";
    const expectedMessage = "Your account has been disabled.";

    component.form.controls.email.setValue(anyValidEmail);
    component.form.controls.password.setValue(anyValidPassword);

    login.mockReturnValue(throwError(() => ({ error: { errorMessage: expectedMessage }, status: 403})));

    component.submit();

    expect(login).toHaveBeenCalledWith({email: anyValidEmail, password: anyValidPassword});
    expect(setSession).toHaveBeenCalledTimes(0);
    expect(navigateByUrl).toHaveBeenCalledTimes(0);
    expect(component.loading()).toBe(false);

    expect(component.errorMessage()).toBe(expectedMessage);
  });

  it("should display a generic error message when login fails unexpectedly", () => {
    const anyValidEmail = "something@email.com";
    const anyValidPassword = "123456";
    const expectedMessage = "An unexpected error occurred.";

    component.form.controls.email.setValue(anyValidEmail);
    component.form.controls.password.setValue(anyValidPassword);

    login.mockReturnValue(throwError(() => ({ status: 500})));

    component.submit();

    expect(login).toHaveBeenCalledWith({email: anyValidEmail, password: anyValidPassword});
    expect(setSession).toHaveBeenCalledTimes(0);
    expect(navigateByUrl).toHaveBeenCalledTimes(0);
    expect(component.loading()).toBe(false);

    expect(component.errorMessage()).toBe(expectedMessage);    
  });

  it("should show the loading state while login is pending", () => {
    const anyValidEmail = "something@email.com";
    const anyValidPassword = "123456";
    const loginSubject = new Subject<LoginResponse>();

    component.form.controls.email.setValue(anyValidEmail);
    component.form.controls.password.setValue(anyValidPassword);

    login.mockReturnValue(loginSubject.asObservable());
    
    component.submit();
    fixture.detectChanges();
    
    const button = fixture.nativeElement.querySelector(".login-button");
        
    expect(component.loading()).toBe(true);
    expect(button.disabled).toBe(true);
    expect(button.textContent).toContain("Logging in ...");
  });

    it("should reset the loading state when the login request completes", () => {
    const anyValidEmail = "something@email.com";
    const anyValidPassword = "123456";
    const loginSubject = new Subject<LoginResponse>();

    component.form.controls.email.setValue(anyValidEmail);
    component.form.controls.password.setValue(anyValidPassword);

    login.mockReturnValue(loginSubject.asObservable());

    component.submit();
    fixture.detectChanges();

    const button = fixture.nativeElement.querySelector(".login-button");

    expect(component.loading()).toBe(true);
    expect(button.textContent).toContain("Logging in ...");

    loginSubject.complete();    
    fixture.detectChanges();
    
    expect(component.loading()).toBe(false);
    expect(button.textContent).toContain("Login");
  });

  it("should display a required validation message when the touched email field is empty", () => {
    component.form.controls.email.markAsTouched();

    fixture.detectChanges();
    const messages = fixture.nativeElement.querySelectorAll("li");

    expect(messages.length).toBe(1);
    expect(messages[0].textContent).toBe("The Email is required.");
  });

  it("should display an email format validation message when the touched email is invalid", () => {
    component.form.controls.email.setValue("not-an-email");
    component.form.controls.email.markAsTouched();

    fixture.detectChanges();
    const messages = fixture.nativeElement.querySelectorAll("li");

    expect(messages.length).toBe(1);
    expect(messages[0].textContent).toBe("Invalid email format.");
  });

  it("should display a required validation message when the touched password field is empty", () => {
    component.form.controls.password.markAsTouched();

    fixture.detectChanges();
    const messages = fixture.nativeElement.querySelectorAll("input[type='password'] ~ ul li");

    expect(messages.length).toBe(1);
    expect(messages[0].textContent).toBe("The Password is required.");
  });

  it("should display a length validation message when the password is too short", () => {
    component.form.controls.password.setValue("12345");
    component.form.controls.password.markAsTouched();

    fixture.detectChanges();
    const messages = fixture.nativeElement.querySelectorAll("input[type='password'] ~ ul li");

    expect(messages.length).toBe(1);
    expect(messages[0].textContent).toBe("The Password must be at least 6 characters.");
  });

  it("should display validation message when the password is dirty but not touched", () => {
    component.form.controls.password.setValue("12345");
    component.form.controls.password.markAsDirty();

    fixture.detectChanges();
    const messages = fixture.nativeElement.querySelectorAll("input[type='password'] ~ ul li");

    expect(messages.length).toBe(1);
    expect(messages[0].textContent).toBe("The Password must be at least 6 characters.");
  });

  it("should not display any validation messages when loaded", () => {
    fixture.detectChanges();
    const messages = fixture.nativeElement.querySelectorAll("li");

    expect(messages.length).toBe(0);
  })
});
