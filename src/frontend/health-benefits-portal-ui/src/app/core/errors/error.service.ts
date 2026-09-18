import { Injectable, signal, inject } from "@angular/core";
import { AppError } from "./error.model";
import { NotificationService } from "../notifications/notification.service";

@Injectable({
    providedIn: "root"
})
export class ErrorService {
    private notificationService = inject(NotificationService);
    private readonly _error = signal<AppError | null>(null);

    readonly error = this._error.asReadonly();
    
    handleError(error: AppError) {
        this._error.set(error);

        this.notificationService.showError(error.message);
    }

    clear() {
        this._error.set(null);
    }
}