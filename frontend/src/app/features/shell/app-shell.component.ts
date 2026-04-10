import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Store } from '@ngrx/store';

import { ClientStorageService } from '../../core/services/client-storage.service';
import { AuthActions, selectSession } from '../../store/auth.store';
import { BookingActions } from '../../store/booking.store';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppShellComponent implements OnInit {
  private readonly store = inject(Store);
  private readonly storage = inject(ClientStorageService);

  readonly session$ = this.store.select(selectSession);

  ngOnInit(): void {
    this.store.dispatch(AuthActions.hydrateSession({ session: this.storage.loadAuthSession() }));
    this.store.dispatch(BookingActions.hydrateSelection({ offer: this.storage.loadSelectedOffer() }));
  }

  logout(): void {
    this.store.dispatch(AuthActions.logoutRequested());
  }
}