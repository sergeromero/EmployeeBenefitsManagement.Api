import { Injectable, signal } from "@angular/core";
import { Notification } from "./notification.model";

@Injectable({
    providedIn: "root"
})
export class NotificationService {
    private readonly _notifications = signal<Notification[]>([]);

    readonly notifications = this._notifications.asReadonly();

    show(notification: Notification) {
        this._notifications.update((list) => [...list, notification]);

        setTimeout(() => {
            this._notifications.update((list) => list.slice(1));
        }, 300);
    }

    showError(message: string) {
        this.show({message, type: "error"});
    }

    showSuccess(message: string) {
        this.show({message, type: "success"});
    }

    showInfo(message: string) {
        this.show({message, type: "info"});
    }
}