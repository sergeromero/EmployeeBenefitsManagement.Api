import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
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
}
