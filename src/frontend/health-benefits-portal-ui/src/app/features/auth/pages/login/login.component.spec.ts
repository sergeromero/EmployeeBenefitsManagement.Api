import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppLogin } from './login.component';
import { AuthApiService } from '@core/api/auth/auth-api.service';
import { AuthService } from '@core/auth/application/auth.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';

describe('AppLogin', () => {
    let component: AppLogin;
    let fixture: ComponentFixture<AppLogin>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [AppLogin],
            providers: [
                {
                    provide: AuthApiService,
                    useValue: {
                        login: vi.fn().mockReturnValue(of({}))
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
    expect(component).toBeTruthy();
  });
});
