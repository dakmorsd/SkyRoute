import { inject, Injectable } from '@angular/core';
import { createActionGroup, createFeature, createReducer, emptyProps, on, props } from '@ngrx/store';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, tap } from 'rxjs';

import { ClientStorageService } from '../core/services/client-storage.service';
import { SkyRouteApiService } from '../core/services/skyroute-api.service';
import { BookingConfirmation, BookingRequest, FlightOffer } from '../shared/models/skyroute.models';
import { extractApiErrorMessage } from '../shared/utils/api-error';

export interface BookingState {
  selectedOffer: FlightOffer | null;
  confirmation: BookingConfirmation | null;
  status: 'idle' | 'submitting' | 'success' | 'error';
  error: string | null;
}

const initialState: BookingState = {
  selectedOffer: null,
  confirmation: null,
  status: 'idle',
  error: null,
};

export const BookingActions = createActionGroup({
  source: 'Booking',
  events: {
    'Hydrate Selection': props<{ offer: FlightOffer | null }>(),
    'Offer Selected': props<{ offer: FlightOffer }>(),
    'Clear Selection': emptyProps(),
    'Booking Requested': props<{ request: BookingRequest }>(),
    'Booking Succeeded': props<{ confirmation: BookingConfirmation }>(),
    'Booking Failed': props<{ message: string }>(),
    'Dismiss Error': emptyProps(),
  },
});

const bookingFeature = createFeature({
  name: 'booking',
  reducer: createReducer(
    initialState,
    on(BookingActions.hydrateSelection, (state, { offer }) => ({
      ...state,
      selectedOffer: offer,
      error: null,
    })),
    on(BookingActions.offerSelected, (_state, { offer }) => ({
      selectedOffer: offer,
      confirmation: null,
      status: 'idle' as BookingState['status'],
      error: null,
    })),
    on(BookingActions.clearSelection, () => initialState),
    on(BookingActions.bookingRequested, state => ({
      ...state,
      status: 'submitting' as BookingState['status'],
      error: null,
      confirmation: null,
    })),
    on(BookingActions.bookingSucceeded, (state, { confirmation }) => ({
      ...state,
      confirmation,
      status: 'success' as BookingState['status'],
      error: null,
    })),
    on(BookingActions.bookingFailed, (state, { message }) => ({
      ...state,
      status: 'error' as BookingState['status'],
      error: message,
    })),
    on(BookingActions.dismissError, state => ({
      ...state,
      error: null,
    })),
  ),
});

export const {
  name: bookingFeatureKey,
  reducer: bookingReducer,
  selectBookingState,
  selectSelectedOffer,
  selectConfirmation,
  selectStatus,
  selectError,
} = bookingFeature;

@Injectable()
export class BookingEffects {
  private readonly actions$ = inject(Actions);
  private readonly api = inject(SkyRouteApiService);
  private readonly storage = inject(ClientStorageService);

  readonly persistOffer$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(BookingActions.offerSelected),
        tap(({ offer }) => this.storage.saveSelectedOffer(offer)),
      ),
    { dispatch: false },
  );

  readonly clearSelection$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(BookingActions.clearSelection),
        tap(() => this.storage.clearSelectedOffer()),
      ),
    { dispatch: false },
  );

  readonly bookingRequested$ = createEffect(() =>
    this.actions$.pipe(
      ofType(BookingActions.bookingRequested),
      exhaustMap(({ request }) =>
        this.api.createBooking(request).pipe(
          map(confirmation => BookingActions.bookingSucceeded({ confirmation })),
          catchError(error => of(BookingActions.bookingFailed({ message: extractApiErrorMessage(error) }))),
        ),
      ),
    ),
  );

  readonly clearStoredOfferOnSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(BookingActions.bookingSucceeded),
        tap(() => this.storage.clearSelectedOffer()),
      ),
    { dispatch: false },
  );
}