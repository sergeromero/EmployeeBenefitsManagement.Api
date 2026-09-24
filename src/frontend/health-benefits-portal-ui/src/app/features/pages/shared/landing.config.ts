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
        label: "Enroll in Benefits",
        description: "Manage your benefit enrollments.",
        route: "/enrollments",
        roles: ["Administrator", "Employee", "HR"]
    },
    {
        id: "manage-employees",
        label: "Manage Employees",
        description: "Create and manage employees.",
        route: "/employees",
        roles: ["Administrator", "HR"]
    },
    {
        id: "admin-user-management",
        label: "User & Role Management",
        description: "Create users and assign roles to employees.",
        route: "/admin/user-management",
        roles: ["Administrator"]
    }
];