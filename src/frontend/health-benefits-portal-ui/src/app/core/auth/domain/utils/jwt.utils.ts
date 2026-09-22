// import { ROLE_CLAIM_KEYS } from "@core/auth/constants/auth.constants";

// export interface JwtPayload {
//     exp: number;
//     role?: string | string [];
//     [key: string]: unknown;
// }

// function base64UrlDecode(base64Url: string): Uint8Array {
//   // Convert from base64url → base64
//   const base64 = base64Url
//     .replace(/-/g, '+')
//     .replace(/_/g, '/')
//     .padEnd(base64Url.length + (4 - (base64Url.length % 4)) % 4, '=');

//   // Decode into binary string
//   const binaryString = globalThis.atob(base64);

//   // Convert to Uint8Array (typed array)
//   const bytes = new Uint8Array(binaryString.length);

//   for (let i = 0; i < binaryString.length; i++) {
//     bytes[i] = binaryString.charCodeAt(i);
//   }

//   return bytes;
// }

// export function decodeJwt(token: string): JwtPayload | null {
//   try {
//     const payloadPart = token.split('.')[1];
//     const bytes = base64UrlDecode(payloadPart);

//     // Decode UTF-8 properly
//     const json = new TextDecoder().decode(bytes);

//     return JSON.parse(json) as JwtPayload;
//   } catch {
//     return null;
//   }
// }

// export function extractRoles(payload: JwtPayload): string[] {
//   let roleClaim: unknown;

//   for (const key of ROLE_CLAIM_KEYS) {
//     if (payload[key] !== undefined) {
//       roleClaim = payload[key];
//       break;
//     }
//   }

//   if (!roleClaim) return [];

//   if (Array.isArray(roleClaim)) {
//     return roleClaim.filter((r): r is string => typeof r === 'string');
//   }

//   if (typeof roleClaim === 'string') {
//     return [roleClaim];
//   }

//   return [];
// }