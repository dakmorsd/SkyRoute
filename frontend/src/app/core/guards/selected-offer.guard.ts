import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

import { ClientStorageService } from '../services/client-storage.service';

export const selectedOfferGuard: CanActivateFn = () => {
  const storage = inject(ClientStorageService);
  const router = inject(Router);

  return storage.loadSelectedOffer() ? true : router.createUrlTree(['/']);
};