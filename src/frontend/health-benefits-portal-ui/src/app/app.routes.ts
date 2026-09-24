import { Routes } from '@angular/router';
import { AppAuthLayout } from './layout/auth-layout/auth-layout.component';
import { AppMainLayout } from './layout/main-layout/main-layout.component';

export const routes: Routes = [
    {
        path: '',
        component: AppMainLayout,
        children: [
            {
                path: '',
                loadComponent: () => import('./features/pages/home/home.component').then(m => m.HomeComponent)
            }
        ]
    },
    {
         path: 'login',
         component: AppAuthLayout,
         children: [
            {
                path: '',
                loadComponent: () => import("./features/auth/pages/login/login.component").then(m => m.AppLogin)
            }
         ]
    },
    {
        path: '**',
        redirectTo: ''
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