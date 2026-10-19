import { DestroyRef, Injectable, inject, signal } from "@angular/core";
import { Notification } from "./notification.model";

const NOTIFICATION_DURATION_MS = 5000;

type NewNotification = Pick<Notification, "message" | "type">;

@Injectable({
    providedIn: "root"
})
export class NotificationService {
    private readonly destroyRef = inject(DestroyRef);
    private readonly _notifications = signal<Notification[]>([]);
    readonly notifications = this._notifications.asReadonly();

    private nextId = 0;
    private readonly timeoutIds = new Map<number, ReturnType<typeof setTimeout>>();

    constructor() {
        this.destroyRef.onDestroy(() => {
            for (const timeoutId of this.timeoutIds.values()) {
                clearTimeout(timeoutId);
            }
            this.timeoutIds.clear();
        })
    }

    show(notification: NewNotification): void {
        const id = ++this.nextId;
        const newNotification: Notification = { ...notification, id };

        this._notifications.update((list) => [...list, newNotification]);

        const timeoutId = setTimeout(
            () => this.dismiss(id),
            NOTIFICATION_DURATION_MS
        );

        this.timeoutIds.set(id, timeoutId);
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

    dismiss(id: number): void {
        const timeoutId = this.timeoutIds.get(id);

        if (timeoutId !== undefined) {
            clearTimeout(timeoutId);
            this.timeoutIds.delete(id);
        }

        this._notifications.update((list) =>
            list.filter((notification) => notification.id !== id)
        );
    }    
}