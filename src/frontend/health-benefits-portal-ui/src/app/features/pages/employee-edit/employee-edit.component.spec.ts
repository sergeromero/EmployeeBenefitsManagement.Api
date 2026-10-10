import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';

import { EmployeeEditComponent } from './employee-edit.component';
import { EmployeeFormComponent } from '../../../shared/components/employee-form/employee-form.component';
import { EmployeesApiService } from '@core/api/employees/employees-api.service';
import { AuthService } from '@core/auth/application/auth.service';

@Component({
    selector: 'app-employee-form',
    standalone: true,
    template: ''
})
class EmployeeFormStub {
    @Input() mode: 'create' | 'edit' = 'create';
    @Input() roles = [];
    @Input() initialData: any | null = null;

    @Output() formSubmit = new EventEmitter<any>();
}

describe('EmployeeEditComponent', () => {
    let component: EmployeeEditComponent;
    let fixture: ComponentFixture<EmployeeEditComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [EmployeeEditComponent],
            providers: [
                {
                    provide: EmployeesApiService,
                    useValue: {
                        getRoles: vi.fn().mockReturnValue(of([])),
                        getById: vi.fn().mockReturnValue(of({
                            employee: {
                                id: 1,
                                employeeNumber: 'EMP001',
                                firstName: 'John',
                                lastName: 'Doe',
                                email: 'john.doe@example.com',
                                hireDate: '2025-01-15',
                                departmentId: 1
                            }
                        })),
                        getCompleteById: vi.fn().mockReturnValue(of({
                            employee: {
                                id: 1,
                                employeeNumber: 'EMP001',
                                firstName: 'John',
                                lastName: 'Doe',
                                email: 'john.doe@example.com',
                                hireDate: '2025-01-15',
                                departmentId: 1
                            },
                            user: {
                                userName: 'john.doe',
                                email: 'john.doe@example.com'
                            },
                            role: 'Employee'
                        })),
                        update: vi.fn(),
                        updateWithUser: vi.fn()
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
                },
                {
                    provide: ActivatedRoute,
                    useValue: {
                        snapshot: {
                            paramMap: {
                                get: vi.fn().mockReturnValue('1')
                            }
                        }
                    }
                }
            ]
        })
        .overrideComponent(EmployeeEditComponent, {
            remove: {
                imports: [EmployeeFormComponent]
            },
            add: {
                imports: [EmployeeFormStub]
            }
        })
        .compileComponents();

        fixture = TestBed.createComponent(EmployeeEditComponent);
        component = fixture.componentInstance;

        await fixture.whenStable();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});