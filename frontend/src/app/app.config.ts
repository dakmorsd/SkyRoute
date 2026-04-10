import { ApplicationConfig, isDevMode, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withInMemoryScrolling } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideEffects } from '@ngrx/effects';
import { provideState, provideStore } from '@ngrx/store';
import { provideStoreDevtools } from '@ngrx/store-devtools';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { AirportsEffects, airportsFeatureKey, airportsReducer } from './store/airports.store';
import { AuthEffects, authFeatureKey, authReducer } from './store/auth.store';
import { BookingEffects, bookingFeatureKey, bookingReducer } from './store/booking.store';
import { SearchEffects, searchFeatureKey, searchReducer } from './store/search.store';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(
      routes,
      withInMemoryScrolling({
        scrollPositionRestoration: 'enabled',
        anchorScrolling: 'enabled',
      }),
    ),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideStore(),
    provideState(authFeatureKey, authReducer),
    provideState(airportsFeatureKey, airportsReducer),
    provideState(searchFeatureKey, searchReducer),
    provideState(bookingFeatureKey, bookingReducer),
    provideEffects([AuthEffects, AirportsEffects, SearchEffects, BookingEffects]),
    provideStoreDevtools({
      maxAge: 25,
      logOnly: !isDevMode(),
    }),
  ]
};
