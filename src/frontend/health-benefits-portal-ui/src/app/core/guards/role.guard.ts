import { inject } from "@angular/core";
import { CanActivateFn, ActivatedRouteSnapshot, Router } from "@angular/router";
import { AuthService } from "@core/auth/application/auth.service";

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (!authService.isAuthenticated()) {
        return router.createUrlTree(['/login']);
    }

    const requiredRoles = route.data?.['roles'] as string[] | undefined;

    if (!requiredRoles || requiredRoles.length == 0) {
        return true;
    }

    const hasAccess = requiredRoles.some(role => authService.hasRole(role));

    if (hasAccess) {
        return true;
    }

    return router.createUrlTree(['/unauthorized']);
}