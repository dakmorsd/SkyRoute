import { FlightOffer } from '../shared/models/skyroute.models';
import { selectSortedOffers } from './search.store';

describe('selectSortedOffers', () => {
  const offers: FlightOffer[] = [
    {
      offerToken: 'offer-1',
      providerCode: 'GLOBAL_AIR',
      providerName: 'GlobalAir',
      flightNumber: 'GA100',
      originAirportCode: 'EZE',
      destinationAirportCode: 'SCL',
      departureTimeUtc: '2026-04-14T08:00:00Z',
      arrivalTimeUtc: '2026-04-14T11:00:00Z',
      durationMinutes: 180,
      cabinClass: 'Economy',
      routeType: 'International',
      pricing: {
        perPassenger: 410,
        passengerCount: 1,
        total: 410,
        currency: 'USD',
      },
    },
    {
      offerToken: 'offer-2',
      providerCode: 'BUDGET_WINGS',
      providerName: 'BudgetWings',
      flightNumber: 'BW200',
      originAirportCode: 'EZE',
      destinationAirportCode: 'SCL',
      departureTimeUtc: '2026-04-14T06:00:00Z',
      arrivalTimeUtc: '2026-04-14T08:20:00Z',
      durationMinutes: 140,
      cabinClass: 'Economy',
      routeType: 'International',
      pricing: {
        perPassenger: 305,
        passengerCount: 1,
        total: 305,
        currency: 'USD',
      },
    },
  ];

  it('preserves original ordering for recommended mode', () => {
    const sorted = selectSortedOffers.projector(offers, 'recommended');

    expect(sorted.map(offer => offer.flightNumber)).toEqual(['GA100', 'BW200']);
  });

  it('sorts by total price on the client when requested', () => {
    const sorted = selectSortedOffers.projector(offers, 'priceAsc');

    expect(sorted.map(offer => offer.flightNumber)).toEqual(['BW200', 'GA100']);
  });
});