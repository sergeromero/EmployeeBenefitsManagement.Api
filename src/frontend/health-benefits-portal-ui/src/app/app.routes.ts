import { Routes } from '@angular/router';
import { AppAuthLayout } from './layout/auth-layout/auth-layout.component';
import { AppMainLayout } from './layout/main-layout/main-layout.component';
import { authGuard } from '@core/guards/auth.guard';

export const routes: Routes = [
    //ROOT ENTRY
    {
        path: '',
        component: AppMainLayout,
        children: [
            {
                path: '',
                loadComponent: () => import('./features/entry/entry.component').then(m => m.EntryComponent)
            }
        ]
    },
    //LOGIN
    {
        path: '',
        component: AppAuthLayout,
        children: [
            {
                path: 'login',
                loadComponent: () => import('./features/auth/pages/login/login.component').then(m => m.AppLogin)
            }
        ]
    },
    //PROTECTED PATHS
    // {
    //     path: '',
    //     component: AppMainLayout,
    //     canActivate: [authGuard],
    //     children: [

    //     {
    //         path: 'benefits',
    //         loadComponent: () =>
    //         import('./features/benefits/benefits.component')
    //             .then(m => m.BenefitsComponent)
    //     },

    //     {
    //         path: 'users',
    //         loadComponent: () =>
    //         import('./features/admin/user-management.component')
    //             .then(m => m.UserManagementComponent),
    //         data: { roles: ['Administrator'] }
    //     }
    //     ]
    // },
    {
        path: '**',
        redirectTo: ''
    }
];


