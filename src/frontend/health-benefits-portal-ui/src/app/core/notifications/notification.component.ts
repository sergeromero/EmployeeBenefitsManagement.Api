import { Component, inject } from "@angular/core";
import { NotificationService } from "./notification.service";

@Component({
    selector: "app-notifications",
    imports: [],
    templateUrl: "notification.component.html",
    styleUrl: "notification.component.scss"
})
export class AppNotification {
    private notificationService = inject(NotificationService);

    readonly notifications = this.notificationService.notifications;

    dismiss(id: number): void {
        this.notificationService.dismiss(id);
    }
}