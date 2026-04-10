import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { map, take } from 'rxjs';

import { ClientStorageService } from '../../core/services/client-storage.service';
import { FlightOffer, SearchCriteria, SortMode } from '../../shared/models/skyroute.models';
import { selectSession } from '../../store/auth.store';
import { AirportsActions, selectError as selectAirportsError, selectItems, selectStatus as selectAirportsStatus } from '../../store/airports.store';
import { BookingActions } from '../../store/booking.store';
import { SearchActions, selectCriteria, selectError as selectSearchError, selectSortedOffers, selectSort, selectStatus as selectSearchStatus } from '../../store/search.store';
import { OffersListComponent } from './offers-list.component';
import { SearchFormComponent } from './search-form.component';
import { SortToolbarComponent } from './sort-toolbar.component';

@Component({
  selector: 'app-search-page',
  standalone: true,
  imports: [CommonModule, RouterLink, SearchFormComponent, SortToolbarComponent, OffersListComponent],
  templateUrl: './search-page.component.html',
  styleUrl: './search-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SearchPageComponent implements OnInit {
  private readonly store = inject(Store);
  private readonly router = inject(Router);
  private readonly storage = inject(ClientStorageService);

  readonly airports$ = this.store.select(selectItems);
  readonly airportsStatus$ = this.store.select(selectAirportsStatus);
  readonly airportsError$ = this.store.select(selectAirportsError);
  readonly searchStatus$ = this.store.select(selectSearchStatus);
  readonly searchError$ = this.store.select(selectSearchError);
  readonly sortedOffers$ = this.store.select(selectSortedOffers);
  readonly sort$ = this.store.select(selectSort);
  readonly criteria$ = this.store.select(selectCriteria);
  readonly session$ = this.store.select(selectSession);
  readonly isAuthenticated$ = this.session$.pipe(map(Boolean));

  ngOnInit(): void {
    this.store
      .select(selectAirportsStatus)
      .pipe(take(1))
      .subscribe(status => {
        if (status === 'idle') {
          this.store.dispatch(AirportsActions.loadRequested());
        }
      });
  }

  search(criteria: SearchCriteria): void {
    this.store.dispatch(SearchActions.searchRequested({ criteria }));
  }

  changeSort(sort: SortMode): void {
    this.store.dispatch(SearchActions.sortChanged({ sort }));
  }

  selectOffer(offer: FlightOffer): void {
    this.storage.saveSelectedOffer(offer);
    this.store.dispatch(BookingActions.offerSelected({ offer }));

    if (this.storage.loadAuthSession()) {
      this.router.navigate(['/checkout']);
      return;
    }

    this.router.navigate(['/auth'], { queryParams: { redirect: '/checkout' } });
  }
}