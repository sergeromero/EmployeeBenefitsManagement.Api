import { Component, Input, Output, EventEmitter, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatCardModule } from '@angular/material/card';
import { AuthService } from '@core/auth/application/auth.service';
import { RoleDto } from '@core/contracts/role/role.dto';

@Component({
  imports:  [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDividerModule,
    MatCardModule
  ],
  selector: 'app-employee-form',
  styleUrl: './employee-form.component.scss',
  templateUrl: './employee-form.component.html',
})
export class EmployeeFormComponent {
  private formBuilder = inject(FormBuilder);
  private auth = inject(AuthService);

  @Input() mode: "create" | "edit" = "create";
  @Input() roles: RoleDto[] = [];
  @Input() initialData: any | null = null;

  @Output() formSubmit = new EventEmitter<FormGroup>();

  loading = signal(false);

  isAdmin = computed(() => this.auth.hasRole("Administrator"));

  form = this.formBuilder.group({
    employee: this.formBuilder.group({
      id: [0],
      employeeNumber: ["", Validators.required],
      firstName: ["", Validators.required],
      lastName: ["", Validators.required],
      email: ["", [Validators.required, Validators.email]],
      hireDate: ["", Validators.required],
      departmentId: [0, Validators.required],
    }),
    user: this.formBuilder.group({
      userName: [""],
      email: [""],
      password: [""]
    }),
    role: [""]
  });

  constructor() {
    this.configureByRole();
  }

  ngOnInit() {
    if (this.initialData) {
      this.patchForm();
    }

    this.configureValidation();
  }

  private configureByRole() {
    if (!this.isAdmin()) {
      this.form.get("user")?.disable();
      this.form.get("role")?.disable();
    }
  }

  private configureValidation() {
    const userGroup = this.form.get("user") as FormGroup;

    if (this.isAdmin()) {
      userGroup.get("userName")?.addValidators(Validators.required);
      userGroup.get("email")?.addValidators([Validators.required, Validators.email]);

      if (this.mode === "create") {
        userGroup.get("password")?.addValidators(Validators.required);
      }
    }
  }

  private patchForm() {
    const data = this.initialData;

    this.form.patchValue({
      employee: {
        id: data.employee.id,
        employeeNumber: data.employee.employeeNumber,
        firstName: data.employee.firstName,
        lastName: data.employee.lastName,
        email: data.employee.email,
        hireDate: data.employee.hireDate,
        departmentId: data.employee.departmentId
      },
      user: data.user ? {
        userName: data.user.userName,
        email: data.user.email,
        password: ""
      } : undefined,
      role: data.role
    });
  }

  submit() {
    if (this.form.invalid) return;
    this.formSubmit.emit(this.form);
  }  
}
