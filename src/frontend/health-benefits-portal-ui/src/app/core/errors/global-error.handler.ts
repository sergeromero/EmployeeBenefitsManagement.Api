import { ErrorHandler, inject, Injectable } from "@angular/core";
import { ErrorService } from "./error.service";
import { HttpErrorResponse } from "@angular/common/http";

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
    private errorService = inject(ErrorService);

    handleError(error: any): void {
        console.error("Unhandled error: ", error);

        if (error instanceof HttpErrorResponse) {
            return;
        }

        if (error?.rejection instanceof HttpErrorResponse) {
            return;
        }

        this.errorService.handleError({
            type: "Unknown",
            message: "An unexpected error occurred."
        })
    }
}