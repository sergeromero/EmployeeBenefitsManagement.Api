import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { EmployeeCreateComponent } from './employee-create.component';
import { EmployeesApiService } from '@core/api/employees/employees-api.service';
import { AuthService } from '@core/auth/application/auth.service';

describe('EmployeeCreateComponent', () => {
  let component: EmployeeCreateComponent;
  let fixture: ComponentFixture<EmployeeCreateComponent>;

  beforeEach(async () => {
      await TestBed.configureTestingModule({
          imports: [EmployeeCreateComponent],
          providers: [
              {
                  provide: EmployeesApiService,
                  useValue: {
                      getRoles: vi.fn().mockReturnValue(of([])),
                      create: vi.fn(),
                      createWithUser: vi.fn()
                  }
              },
              {
                  provide: AuthService,
                  useValue: {
                      hasRole: vi.fn().mockReturnValue(false)
                  }
              },
              {
                  provide: Router,
                  useValue: {
                      navigate: vi.fn()
                  }
              }
          ]
      }).compileComponents();

      fixture = TestBed.createComponent(EmployeeCreateComponent);
      component = fixture.componentInstance;

      await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
