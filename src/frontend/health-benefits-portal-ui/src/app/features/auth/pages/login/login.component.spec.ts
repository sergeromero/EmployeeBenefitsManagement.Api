import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppLogin } from './login.component';
import { AuthApiService } from '@core/api/auth/auth-api.service';
import { AuthService } from '@core/auth/application/auth.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';

describe('AppLogin', () => {
    let component: AppLogin;
    let fixture: ComponentFixture<AppLogin>;
    let login: ReturnType<typeof vi.fn>;

    beforeEach(async () => {
        login = vi.fn();

        await TestBed.configureTestingModule({
            imports: [AppLogin],
            providers: [
                {
                    provide: AuthApiService,
                    useValue: {
                        login: login
                    }
                },
                {
                    provide: AuthService,
                    useValue: {
                        setSession: vi.fn()
                    }
                },
                {
                    provide: Router,
                    useValue: {
                        navigateByUrl: vi.fn()
                    }
                }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(AppLogin);
        component = fixture.componentInstance;

        await fixture.whenStable();
    });

  it('should create', () => {
    login.mockReturnValue(of({}));
    expect(component).toBeTruthy();
  });

  it("should mark all controls as touched and not attempt to login when the form is invalid", () => {
    component.submit();

    expect(component.form.controls.email.touched).toBe(true);
    expect(component.form.controls.password.touched).toBe(true);
    expect(login).toHaveBeenCalledTimes(0);
  })
});
