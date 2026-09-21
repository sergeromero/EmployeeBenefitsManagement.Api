import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, ValidationErrors, AbstractControl } from '@angular/forms';
import { AuthApiService } from '@core/api/auth/auth-api.service';
import { createLoginForm } from './login.form';
import { finalize } from 'rxjs';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule],
  selector: 'app-login',
  styleUrl: './login.component.scss',
  templateUrl: './login.component.html',
})
export class AppLogin {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authApi = inject(AuthApiService);

  readonly form = createLoginForm(this.formBuilder);

  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  submit(): void {
    if(this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.errorMessage.set(null);
    this.loading.set(true);

    this.authApi.login(this.form.getRawValue())
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (response) => {
          console.log('JWT:', response.accessToken);
          //Storage on story 2.2
        },
        error: (err) => {
          this.errorMessage.set(this.extractError(err));
        }
      });
  }

  private extractError(error: any): string {
    if (error?.error?.errorMessage){
      return error.error.errorMessage;
    }

    if(error?.status === 401) {
      return "Invalid email or password.";
    }

    return "An unexpected error occurred.";
  }

  getMessages(errs: ValidationErrors | null, name: string): string[] {
    let messages: string[] = [];
    if (!errs) return messages;

    for (let errorName in errs) {
      switch (errorName) {
        case "required":
          messages.push(`The ${name} is required.`);
          break;
        case "minlength":
          messages.push(`The ${name} must be at least ${errs['minlength'].requiredLength} characters.`);
          break;
        case "email":
          messages.push(`Invalid email format.`);
          break;
      }
    }
    return messages;
  }

  getValidationMessages(control: AbstractControl, thingName: string) {
    return this.getMessages(control.errors, thingName)
  }
}
