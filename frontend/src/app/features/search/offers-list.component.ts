import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Airport, FlightOffer } from '../../shared/models/skyroute.models';

@Component({
  selector: 'app-offers-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './offers-list.component.html',
  styleUrl: './offers-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OffersListComponent {
  @Input() offers: FlightOffer[] = [];
  @Input() airports: Airport[] = [];
  @Input() isAuthenticated = false;

  @Output() readonly offerSelected = new EventEmitter<FlightOffer>();

  describeAirport(code: string): string {
    const airport = this.airports.find(entry => entry.code === code);
    return airport ? `${airport.city} (${airport.code})` : code;
  }
}