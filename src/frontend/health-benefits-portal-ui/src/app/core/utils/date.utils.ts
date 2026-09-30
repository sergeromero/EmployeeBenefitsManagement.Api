export function parseDateOnly(value: string): string {
  if (!value) {
    throw new Error('Cannot convert an empty string to date.');
  }

  // Strict format check: yyyy-MM-dd
  const regex = /^\d{4}-\d{2}-\d{2}$/;

  if (!regex.test(value)) {
    throw new Error(`Invalid date format: ${value}`);
  }

  const date = new Date(value);

  if (isNaN(date.getTime())) {
    throw new Error(`Invalid date value: ${value}`);
  }

  // Return EXACT same string (no transformation!)
  return value;
}