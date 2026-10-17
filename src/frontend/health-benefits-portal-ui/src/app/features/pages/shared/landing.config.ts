import { LandingAction } from "./landing.model";

export const LANDING_ACTIONS: LandingAction[] = [
    {
        id: "view-benefits",
        label: "View Benefits",
        description: "View available benefit plans.",
        route: "/benefits",
        roles: ["Administrator", "Employee", "HR"]
    },
    {
        id: "enroll-benefits",
        label: "My enrollments",
        description: "Manage your current benefit enrollments.",
        route: "employees/my/enrollments",
        roles: ["Administrator", "Employee", "HR"]
    },
    {
        id: "employees-group",
        label: "Employees",
        description: "Employee management",
        roles: ["Administrator", "HR"],
        children: [
            {
                id: "list-employees",
                label: "Employee Lookup",
                description: "Employee Lookup",
                route: "/employees",
                roles: ["Administrator", "HR"]
            },
            {
                id: "create-employees",
                label: "Create Employees",
                description: "Adding new employees.",
                route: "/employees/create",
                roles: ["Administrator", "HR"]
            },
        ]
    },

    {
        id: "admin-user-management",
        label: "User & Role Management",
        description: "Create users and assign roles to employees.",
        route: "/admin/user-management",
        roles: ["Administrator"]
    }
];