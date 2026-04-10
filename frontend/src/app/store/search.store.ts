import { inject, Injectable } from '@angular/core';
import { createActionGroup, createFeature, createReducer, on, props, createSelector } from '@ngrx/store';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, of, switchMap } from 'rxjs';

import { SkyRouteApiService } from '../core/services/skyroute-api.service';
import { FlightOffer, SearchCriteria, SortMode } from '../shared/models/skyroute.models';
import { extractApiErrorMessage } from '../shared/utils/api-error';

export interface SearchState {
  criteria: SearchCriteria | null;
  offers: FlightOffer[];
  status: 'idle' | 'loading' | 'success' | 'error';
  sort: SortMode;
  error: string | null;
}

const initialState: SearchState = {
  criteria: null,
  offers: [],
  status: 'idle',
  sort: 'recommended',
  error: null,
};

export const SearchActions = createActionGroup({
  source: 'Search',
  events: {
    'Search Requested': props<{ criteria: SearchCriteria }>(),
    'Search Succeeded': props<{ criteria: SearchCriteria; offers: FlightOffer[] }>(),
    'Search Failed': props<{ message: string }>(),
    'Sort Changed': props<{ sort: SortMode }>(),
    'Reset Results': props<{ criteria?: SearchCriteria | null }>(),
  },
});

const searchFeature = createFeature({
  name: 'search',
  reducer: createReducer(
    initialState,
    on(SearchActions.searchRequested, (state, { criteria }) => ({
      ...state,
      criteria,
      offers: [],
      status: 'loading' as SearchState['status'],
      error: null,
    })),
    on(SearchActions.searchSucceeded, (state, { criteria, offers }) => ({
      ...state,
      criteria,
      offers,
      status: 'success' as SearchState['status'],
      error: null,
    })),
    on(SearchActions.searchFailed, (state, { message }) => ({
      ...state,
      status: 'error' as SearchState['status'],
      error: message,
    })),
    on(SearchActions.sortChanged, (state, { sort }) => ({
      ...state,
      sort,
    })),
    on(SearchActions.resetResults, (state, { criteria }) => ({
      ...state,
      criteria: criteria ?? state.criteria,
      offers: [],
      status: 'idle' as SearchState['status'],
      error: null,
    })),
  ),
});

export const {
  name: searchFeatureKey,
  reducer: searchReducer,
  selectSearchState,
  selectCriteria,
  selectOffers,
  selectStatus,
  selectSort,
  selectError,
} = searchFeature;

export const selectSortedOffers = createSelector(selectOffers, selectSort, (offers, sort) => {
  const sorted = [...offers];

  switch (sort) {
    case 'priceAsc':
      return sorted.sort((left, right) => left.pricing.totalPrice - right.pricing.totalPrice);
    case 'priceDesc':
      return sorted.sort((left, right) => right.pricing.totalPrice - left.pricing.totalPrice);
    case 'durationAsc':
      return sorted.sort((left, right) => left.durationMinutes - right.durationMinutes);
    case 'departureAsc':
      return sorted.sort(
        (left, right) => new Date(left.departureTimeUtc).getTime() - new Date(right.departureTimeUtc).getTime(),
      );
    default:
      return sorted;
  }
});

@Injectable()
export class SearchEffects {
  private readonly actions$ = inject(Actions);
  private readonly api = inject(SkyRouteApiService);

  readonly search$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SearchActions.searchRequested),
      switchMap(({ criteria }) =>
        this.api.searchFlights(criteria).pipe(
          map(response => SearchActions.searchSucceeded({ criteria, offers: response.offers })),
          catchError(error => of(SearchActions.searchFailed({ message: extractApiErrorMessage(error) }))),
        ),
      ),
    ),
  );
}