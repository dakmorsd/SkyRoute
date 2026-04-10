import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

import { ClientStorageService } from '../services/client-storage.service';

export const authGuard: CanActivateFn = (_route, state) => {
  const storage = inject(ClientStorageService);
  const router = inject(Router);

  return storage.loadAuthSession()
    ? true
    : router.createUrlTree(['/auth'], { queryParams: { redirect: state.url } });
};