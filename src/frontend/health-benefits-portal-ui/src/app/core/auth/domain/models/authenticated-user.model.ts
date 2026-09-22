export interface AuthenticatedUser {
    id: string;
    email: string;
    roles: string[];
    accessToken: string;
    expiresAt: number;
}