import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import {
  Airport,
  AuthSession,
  BookingConfirmation,
  BookingRequest,
  LoginPayload,
  RegisterPayload,
  SearchCriteria,
  SearchResponse,
} from '../../shared/models/skyroute.models';

@Injectable({ providedIn: 'root' })
export class SkyRouteApiService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = 'http://localhost:5279/api';

  getAirports(): Observable<Airport[]> {
    return this.http.get<Airport[]>(`${this.apiBaseUrl}/airports`);
  }

  register(payload: RegisterPayload): Observable<AuthSession> {
    return this.http.post<AuthSession>(`${this.apiBaseUrl}/auth/register`, payload);
  }

  login(payload: LoginPayload): Observable<AuthSession> {
    return this.http.post<AuthSession>(`${this.apiBaseUrl}/auth/login`, payload);
  }

  searchFlights(payload: SearchCriteria): Observable<SearchResponse> {
    return this.http.post<SearchResponse>(`${this.apiBaseUrl}/flights/search`, payload);
  }

  createBooking(payload: BookingRequest): Observable<BookingConfirmation> {
    return this.http.post<BookingConfirmation>(`${this.apiBaseUrl}/bookings`, payload);
  }
}