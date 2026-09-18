export type ErrorType =
  | 'Validation'
  | 'Unauthorized'
  | 'Forbidden'
  | 'NotFound'
  | 'Server'
  | 'Unknown';

  export interface AppError {
    type: ErrorType;
    message: string,
    details?: string[];
    statusCode?: number;
  }