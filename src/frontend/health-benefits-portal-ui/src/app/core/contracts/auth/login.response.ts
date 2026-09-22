export interface LoginResponse {
    accessToken: string;
    expiresAt: string;
    user: {
        id: string;
        email: string;
        roles: string[];
    }
}