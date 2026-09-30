import { Routes } from '@angular/router';
import { AppAuthLayout } from './layout/auth-layout/auth-layout.component';
import { AppMainLayout } from './layout/main-layout/main-layout.component';
import { authGuard } from '@core/guards/auth.guard';

export const routes: Routes = [    
    //LOGIN
    {
        path: 'login',
        component: AppAuthLayout,
        children: [
            {
                path: '',
                loadComponent: () => import('./features/auth/pages/login/login.component').then(m => m.AppLogin)
            }
        ]
    },
    //ROOT PUBLIC + AUTH-AWARE ROOT
    {
        path: '',
        component: AppMainLayout,
        children: [
            {
                path: '',
                pathMatch: "full",
                loadComponent: () => import('./features/entry/entry.component').then(m => m.EntryComponent)
            },
            //RESTRICTED - AUTHENTICATED USERS ONLY
            {
                path: 'employees',
                canActivate: [authGuard],
                loadComponent: () => import("./features/pages/employee-shell/employee-shell.component").then(m => m.EmployeeShellComponent),
                data: { roles: ["Administrator", "HR"]},
                children: [
                    {
                        path: '',
                        loadComponent: () => import("./features/pages/employee-list/employee-list.component").then(m => m.EmployeeListComponent)
                    },
                    {
                        path: 'create',
                        loadComponent: () => import("./features/pages/employee-create/employee-create.component").then(m => m.EmployeeCreateComponent)
                    }
                ]
            },
            {
                path: "unauthorized",
                loadComponent: () => import("./features/pages/unauthorized/unauthorized.component").then(m => m.Unauthorized),
            },
            {
                path: '**',
                loadComponent: () => import("./features/pages/not-found/not-found.component").then(m => m.NotFoundComponent)
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
];


