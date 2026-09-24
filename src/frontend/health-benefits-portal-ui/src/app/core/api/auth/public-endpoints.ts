import { APP_CONFIG } from "@core/config/app-config.token";
import { inject } from "@angular/core";

type EndpointMatcher = string | RegExp | ((url: string) => boolean);

export const getPublicEndpoints = (): EndpointMatcher[] => {
  const appConfig = inject(APP_CONFIG);
  return [
    appConfig.loginUrl,

  // Pattern match example (if needed later)
  // /^\/api\/public\//,

  // Functional matcher (max flexibility)
  // (url) => url.includes('/health-check')
  ];
};