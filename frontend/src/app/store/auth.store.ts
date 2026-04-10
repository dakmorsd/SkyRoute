import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { createActionGroup, createFeature, createReducer, emptyProps, on, props } from '@ngrx/store';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, of, switchMap, tap } from 'rxjs';

import { ClientStorageService } from '../core/services/client-storage.service';
import { SkyRouteApiService } from '../core/services/skyroute-api.service';
import { AuthSession, LoginPayload, RegisterPayload } from '../shared/models/skyroute.models';
import { extractApiErrorMessage } from '../shared/utils/api-error';

export interface AuthState {
  session: AuthSession | null;
  status: 'idle' | 'submitting' | 'authenticated';
  error: string | null;
}

const initialState: AuthState = {
  session: null,
  status: 'idle',
  error: null,
};

export const AuthActions = createActionGroup({
  source: 'Auth',
  events: {
    'Hydrate Session': props<{ session: AuthSession | null }>(),
    'Login Requested': props<{ credentials: LoginPayload }>(),
    'Register Requested': props<{ registration: RegisterPayload }>(),
    'Auth Succeeded': props<{ session: AuthSession }>(),
    'Auth Failed': props<{ message: string }>(),
    'Logout Requested': emptyProps(),
    'Logout Completed': emptyProps(),
    'Clear Error': emptyProps(),
  },
});

const authFeature = createFeature({
  name: 'auth',
  reducer: createReducer(
    initialState,
    on(AuthActions.hydrateSession, (state, { session }) => ({
      ...state,
      session,
      status: (session ? 'authenticated' : 'idle') as AuthState['status'],
      error: null,
    })),
    on(AuthActions.loginRequested, AuthActions.registerRequested, state => ({
      ...state,
      status: 'submitting' as AuthState['status'],
      error: null,
    })),
    on(AuthActions.authSucceeded, (_state, { session }) => ({
      session,
      status: 'authenticated' as AuthState['status'],
      error: null,
    })),
    on(AuthActions.authFailed, (state, { message }) => ({
      ...state,
      status: (state.session ? 'authenticated' : 'idle') as AuthState['status'],
      error: message,
    })),
    on(AuthActions.logoutCompleted, () => initialState),
    on(AuthActions.clearError, state => ({
      ...state,
      error: null,
    })),
  ),
});

export const {
  name: authFeatureKey,
  reducer: authReducer,
  selectAuthState,
  selectSession,
  selectStatus,
  selectError,
} = authFeature;

@Injectable()
export class AuthEffects {
  private readonly actions$ = inject(Actions);
  private readonly api = inject(SkyRouteApiService);
  private readonly storage = inject(ClientStorageService);
  private readonly router = inject(Router);

  readonly login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.loginRequested),
      switchMap(({ credentials }) =>
        this.api.login(credentials).pipe(
          map(session => AuthActions.authSucceeded({ session })),
          catchError(error => of(AuthActions.authFailed({ message: extractApiErrorMessage(error) }))),
        ),
      ),
    ),
  );

  readonly register$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.registerRequested),
      switchMap(({ registration }) =>
        this.api.register(registration).pipe(
          map(session => AuthActions.authSucceeded({ session })),
          catchError(error => of(AuthActions.authFailed({ message: extractApiErrorMessage(error) }))),
        ),
      ),
    ),
  );

  readonly persistSession$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.authSucceeded),
        tap(({ session }) => this.storage.saveAuthSession(session)),
      ),
    { dispatch: false },
  );

  readonly logout$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.logoutRequested),
      tap(() => this.storage.clearAuthSession()),
      tap(() => this.router.navigateByUrl('/')),
      map(() => AuthActions.logoutCompleted()),
    ),
  );
}