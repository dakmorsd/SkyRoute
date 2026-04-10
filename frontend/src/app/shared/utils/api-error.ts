import { HttpErrorResponse } from '@angular/common/http';

import { ApiProblem } from '../models/skyroute.models';

function isApiProblem(value: unknown): value is ApiProblem {
  if (!value || typeof value !== 'object') {
    return false;
  }

  const candidate = value as ApiProblem;
  return typeof candidate.title === 'string' || typeof candidate.errors === 'object';
}

export function extractApiErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    if (isApiProblem(error.error)) {
      const detailMessages = Object.values(error.error.errors ?? {}).flat();

      if (detailMessages.length > 0) {
        return detailMessages.join(' ');
      }

      if (error.error.title) {
        return error.error.title;
      }
    }

    return error.message || 'The request could not be completed.';
  }

  if (error instanceof Error) {
    return error.message;
  }

  return 'The request could not be completed.';
}