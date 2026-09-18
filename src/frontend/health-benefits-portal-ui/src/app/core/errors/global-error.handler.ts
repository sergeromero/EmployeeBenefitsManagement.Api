import { ErrorHandler, inject, Injectable } from "@angular/core";
import { ErrorService } from "./error.service";

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
    private errorService = inject(ErrorService);

    handleError(error: any): void {
        console.error("Unhandled error: ", error);

        this.errorService.handleError({
            type: "Unknown",
            message: "An unexpected error occurred."
        })
    }
}