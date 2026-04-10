import { RouteType } from '../models/skyroute.models';

export interface DocumentRule {
  label: string;
  placeholder: string;
  hint: string;
  pattern: string;
}

const passportPattern = '^[A-Z0-9]{6,9}$';
const nationalIdPattern = '^\\d{7,8}$';

export function getDocumentRule(routeType: RouteType): DocumentRule {
  if (routeType === 'International') {
    return {
      label: 'Passport Number',
      placeholder: 'Example: AR1290ZX',
      hint: '6 to 9 uppercase letters or numbers.',
      pattern: passportPattern,
    };
  }

  return {
    label: 'National ID',
    placeholder: 'Example: 12345678',
    hint: '7 to 8 digits for domestic routes.',
    pattern: nationalIdPattern,
  };
}