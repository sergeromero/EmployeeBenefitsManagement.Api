import { HttpErrorResponse } from "@angular/common/http";
import { AppError } from "./error.model";

export function mapHttpError(error: HttpErrorResponse): AppError {
    if (error.status === 0) {
        return {
            type: "Unknown",
            message: "Network error: Please check your connection."
        };
    }

    switch (error.status) {
        case 400:
            return {
                type: "Validation",
                message: "Invalid request",
                details: extractValidationErrors(error),
                statusCode: 400
            }
        case 401:
            return {
                type: "Unauthorized",
                message: "You must log in to continue",
                statusCode: 401
            }
        case 403:
            return {
                type: "Forbidden",
                message: "You do not have permissions.",
                statusCode: 403
            }
        case 404:
            return {
                type: "NotFound",
                message: "Resource not found.",
                statusCode: 404
            }
        case 500:
            return {
                type: "Server",
                message: "Something went wrong on the server.",
                statusCode: 500
            }
        default:
            return {
                type: "Unknown",
                message: "An unexpected error occurred. Please try again.",
                statusCode: error.status
            }
    }
}

function extractValidationErrors(error: HttpErrorResponse): string[] {
    if (error.error?.errors) {
        return (Object.values(error.error.errors) as string[][]).flat();
    }
    return [];
}