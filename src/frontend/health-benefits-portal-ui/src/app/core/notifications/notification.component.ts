import { Component, computed, inject } from "@angular/core";
import { CommonModule } from "@angular/common";
import { NotificationService } from "./notification.service";

@Component({
    selector: "app-notifications",
    imports: [CommonModule],
    templateUrl: "notification.component.html",
    styleUrl: "notification.component.scss"
})
export class AppNotification {
    private notificationService = inject(NotificationService);

    readonly notifications = this.notificationService.notifications;
}