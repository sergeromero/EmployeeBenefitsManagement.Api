import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '@core/auth/application/auth.service';
import { catchError, throwError } from 'rxjs';
import { getPublicEndpoints } from '@core/api/auth/public-endpoints';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.token();

  if (isPUblicEndpoint(req.url)) {
    return next(req);
  }

  const authReq = token 
  ? req.clone({
    setHeaders: { Authorization: `Bearer ${token}`}
  })
  : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        authService.clear();
      }

      return throwError(() => error);
    })
  );
};

function isPUblicEndpoint(url: string): boolean {
  return getPublicEndpoints().some((matcher) => {
    if (typeof matcher === 'string') {
      return url.includes(matcher);
    }

    if (matcher instanceof RegExp) {
      return matcher.test(url);
    }

    if (typeof matcher === 'function') {
      return matcher(url);
    }

    return false;
  });
}