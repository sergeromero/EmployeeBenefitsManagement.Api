import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';

import { EmployeeListComponent } from './employee-list.component';
import { EmployeesApiService } from '@core/api/employees/employees-api.service';
import { AuthService } from '@core/auth/application/auth.service';

describe('EmployeeListComponent', () => {
    let component: EmployeeListComponent;
    let fixture: ComponentFixture<EmployeeListComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [EmployeeListComponent],
            providers: [
                {
                    provide: EmployeesApiService,
                    useValue: {
                        getAll: vi.fn().mockReturnValue(
                            of({
                                items: [],
                                totalCount: 0
                            })
                        )
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

        fixture = TestBed.createComponent(EmployeeListComponent);
        component = fixture.componentInstance;

        await fixture.whenStable();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});