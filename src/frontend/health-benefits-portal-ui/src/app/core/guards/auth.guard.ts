import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthService } from "@core/auth/application/auth.service";

export const authGuard: CanActivateFn = (route) => {
    const authService = inject(AuthService);
    const router = inject(Router);
    const user = authService.user();

    if (!authService.isAuthenticated()) {
        return router.parseUrl("/login");
    }

    const requiredRoles = route.data?.["roles"] as string[] | undefined;

    if (requiredRoles && !requiredRoles.some(r => user?.roles.includes(r))) {
        return router.parseUrl("/unauthorized");
    }

    return true;
};