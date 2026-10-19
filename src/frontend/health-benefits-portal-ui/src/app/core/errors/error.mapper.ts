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
        case 409:
            return {
                type: "Conflict",
                message: extractProblemDetailsDetail(error) ?? "The requested operation conflicts with the current state.",
                statusCode: 409
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

function extractProblemDetailsDetail(error: HttpErrorResponse): string | null {
    const body: unknown = error.error;

    if (typeof body !== "object" || body === null || !("detail" in body)) {
        return null;
    }

    const detail = body.detail;

    return typeof detail === "string" && detail.trim().length > 0 ? detail : null;
}

function extractValidationErrors(error: HttpErrorResponse): string[] {
    const body: unknown = error.error;

    if (typeof body !== "object" || body === null || !("errors" in body)) {
        return [];
    }
     const errors = body.errors;

    if (typeof errors !== "object" || errors === null) {
        return [];
    }

    return Object.values(errors).filter((value): value is string[] =>
        Array.isArray(value) && value.every((item): item is string => typeof item === "string")).flat();
}