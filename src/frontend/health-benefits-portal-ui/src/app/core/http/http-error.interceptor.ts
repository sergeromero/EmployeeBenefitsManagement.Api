import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, throwError } from "rxjs";
import { ErrorService } from "../errors/error.service";
import { mapHttpError } from "../errors/error.mapper";

export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
    const errorService = inject(ErrorService);

    return next(req).pipe(
        catchError((error) => {
            const appError = mapHttpError(error);
            errorService.handleError(appError);
            return throwError(() => appError);
        })
    );
}