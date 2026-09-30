import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EmployeeShellComponent } from './employee-shell.component';

describe('EmployeeComponent', () => {
  let component: EmployeeShellComponent;
  let fixture: ComponentFixture<EmployeeShellComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmployeeShellComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(EmployeeShellComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
