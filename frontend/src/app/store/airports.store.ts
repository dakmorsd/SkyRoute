import { inject, Injectable } from '@angular/core';
import { createActionGroup, createFeature, createReducer, emptyProps, on, props } from '@ngrx/store';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of } from 'rxjs';

import { SkyRouteApiService } from '../core/services/skyroute-api.service';
import { Airport } from '../shared/models/skyroute.models';
import { extractApiErrorMessage } from '../shared/utils/api-error';

export interface AirportsState {
  items: Airport[];
  status: 'idle' | 'loading' | 'loaded' | 'error';
  error: string | null;
}

const initialState: AirportsState = {
  items: [],
  status: 'idle',
  error: null,
};

export const AirportsActions = createActionGroup({
  source: 'Airports',
  events: {
    'Load Requested': emptyProps(),
    'Load Succeeded': props<{ items: Airport[] }>(),
    'Load Failed': props<{ message: string }>(),
  },
});

const airportsFeature = createFeature({
  name: 'airports',
  reducer: createReducer(
    initialState,
    on(AirportsActions.loadRequested, state => ({
      ...state,
      status: 'loading' as AirportsState['status'],
      error: null,
    })),
    on(AirportsActions.loadSucceeded, (_state, { items }) => ({
      items,
      status: 'loaded' as AirportsState['status'],
      error: null,
    })),
    on(AirportsActions.loadFailed, (state, { message }) => ({
      ...state,
      status: 'error' as AirportsState['status'],
      error: message,
    })),
  ),
});

export const {
  name: airportsFeatureKey,
  reducer: airportsReducer,
  selectAirportsState,
  selectItems,
  selectStatus,
  selectError,
} = airportsFeature;

@Injectable()
export class AirportsEffects {
  private readonly actions$ = inject(Actions);
  private readonly api = inject(SkyRouteApiService);

  readonly load$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AirportsActions.loadRequested),
      exhaustMap(() =>
        this.api.getAirports().pipe(
          map(items => AirportsActions.loadSucceeded({ items })),
          catchError(error => of(AirportsActions.loadFailed({ message: extractApiErrorMessage(error) }))),
        ),
      ),
    ),
  );
}