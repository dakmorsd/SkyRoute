import { inject, Injectable, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

import { AuthSession, FlightOffer } from '../../shared/models/skyroute.models';

@Injectable({ providedIn: 'root' })
export class ClientStorageService {
  private readonly platformId = inject(PLATFORM_ID);
  private readonly authStorageKey = 'skyroute.auth-session';
  private readonly offerStorageKey = 'skyroute.selected-offer';

  loadAuthSession(): AuthSession | null {
    const raw = this.readFromLocalStorage(this.authStorageKey);

    if (!raw) {
      return null;
    }

    try {
      const session = JSON.parse(raw) as AuthSession;
      const isExpired = new Date(session.expiresAtUtc).getTime() <= Date.now();

      if (isExpired) {
        this.clearAuthSession();
        return null;
      }

      return session;
    } catch {
      this.clearAuthSession();
      return null;
    }
  }

  saveAuthSession(session: AuthSession): void {
    this.writeToLocalStorage(this.authStorageKey, JSON.stringify(session));
  }

  clearAuthSession(): void {
    this.removeFromLocalStorage(this.authStorageKey);
  }

  loadSelectedOffer(): FlightOffer | null {
    const raw = this.readFromSessionStorage(this.offerStorageKey);

    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as FlightOffer;
    } catch {
      this.clearSelectedOffer();
      return null;
    }
  }

  saveSelectedOffer(offer: FlightOffer): void {
    this.writeToSessionStorage(this.offerStorageKey, JSON.stringify(offer));
  }

  clearSelectedOffer(): void {
    this.removeFromSessionStorage(this.offerStorageKey);
  }

  private readFromLocalStorage(key: string): string | null {
    if (!isPlatformBrowser(this.platformId)) {
      return null;
    }

    return localStorage.getItem(key);
  }

  private writeToLocalStorage(key: string, value: string): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    localStorage.setItem(key, value);
  }

  private removeFromLocalStorage(key: string): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    localStorage.removeItem(key);
  }

  private readFromSessionStorage(key: string): string | null {
    if (!isPlatformBrowser(this.platformId)) {
      return null;
    }

    return sessionStorage.getItem(key);
  }

  private writeToSessionStorage(key: string, value: string): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    sessionStorage.setItem(key, value);
  }

  private removeFromSessionStorage(key: string): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    sessionStorage.removeItem(key);
  }
}