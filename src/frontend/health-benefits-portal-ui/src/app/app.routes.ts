import { Routes } from '@angular/router';

export const routes: Routes = [
    {
         path: 'login',
         loadComponent: () => import("./features/auth/pages/login/login.component").then(m => m.AppLogin)
    },
    {
        path: 'unauthorized',
        loadComponent: () => import("./features/shared/unauthorized/unauthorized.component").then(m => m.Unauthorized)
    }
];


//EXAMPLES FOR USING THE USER AND ROLE GUARDS
//   {
//     path: 'dashboard',
//     canActivate: [authGuard],
//     loadComponent: () =>
//       import('./features/dashboard/dashboard.component')
//         .then(m => m.DashboardComponent)
//   },
//   {
//     path: 'admin',
//     canActivate: [authGuard, roleGuard],
//     data: { roles: ['Administrator'] },
//     loadComponent: () =>
//       import('./features/admin/admin.component')
//         .then(m => m.AdminComponent)
//   }