export type UserRole = "Administrator" | "HR" | "Employee";

export interface LandingAction {
    id: string;
    label: string;
    description: string;
    route: string;
    roles: UserRole[];
}