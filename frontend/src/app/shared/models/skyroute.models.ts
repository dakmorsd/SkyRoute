export type CabinClass = 'Economy' | 'Business' | 'FirstClass';
export type RouteType = 'Domestic' | 'International';
export type SortMode = 'recommended' | 'priceAsc' | 'priceDesc' | 'durationAsc' | 'departureAsc';

export interface Airport {
  code: string;
  name: string;
  city: string;
  countryCode: string;
  countryName: string;
}

export interface UserProfile {
  id: string;
  fullName: string;
  email: string;
}

export interface AuthSession {
  token: string;
  expiresAtUtc: string;
  user: UserProfile;
}

export interface LoginPayload {
  email: string;
  password: string;
}

export interface RegisterPayload {
  fullName: string;
  email: string;
  password: string;
}

export interface SearchCriteria {
  originAirportCode: string;
  destinationAirportCode: string;
  departureDate: string;
  passengerCount: number;
  cabinClass: CabinClass;
}

export interface PriceBreakdown {
  perPassenger: number;
  passengerCount: number;
  total: number;
  currency: string;
}

export interface FlightOffer {
  offerToken: string;
  providerCode: string;
  providerName: string;
  flightNumber: string;
  originAirportCode: string;
  destinationAirportCode: string;
  departureTimeUtc: string;
  arrivalTimeUtc: string;
  durationMinutes: number;
  cabinClass: CabinClass;
  routeType: RouteType;
  pricing: PriceBreakdown;
}

export interface SearchResponse {
  offers: FlightOffer[];
}

export interface BookingPassenger {
  fullName: string;
  email: string;
  documentNumber: string;
}

export interface BookingRequest {
  offerToken: string;
  passengers: BookingPassenger[];
}

export interface BookingConfirmation {
  bookingReferenceCode: string;
  providerCode: string;
  flightNumber: string;
  routeType: RouteType;
  passengerCount: number;
  perPassengerPrice: number;
  totalPrice: number;
}

export interface ApiProblem {
  title?: string;
  status?: number;
  errors?: Record<string, string[]>;
}