import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';

import { ClientStorageService } from '../services/client-storage.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const storage = inject(ClientStorageService);
  const session = storage.loadAuthSession();

  if (!session?.token) {
    return next(request);
  }

  return next(
    request.clone({
      setHeaders: {
        Authorization: `Bearer ${session.token}`,
      },
    }),
  );
};