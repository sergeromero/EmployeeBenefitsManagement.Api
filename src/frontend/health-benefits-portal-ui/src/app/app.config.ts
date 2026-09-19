import { ApplicationConfig, ErrorHandler, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import { APP_CONFIG } from './core/config/app-config.token';
import { httpErrorInterceptor } from './core/http/http-error.interceptor';
import { GlobalErrorHandler } from './core/errors/global-error.handler';
import { authInterceptor } from './core/interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes), 
    provideClientHydration(),
    provideHttpClient(withInterceptors([authInterceptor, httpErrorInterceptor])),
    {
      provide: APP_CONFIG,
      useValue: {
        apiBaseUrl: "https://localhost:7129/api"
      }
    },
    {
      provide: ErrorHandler,
      useClass: GlobalErrorHandler
    }
  ]
};
