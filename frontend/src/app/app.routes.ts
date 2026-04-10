import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { selectedOfferGuard } from './core/guards/selected-offer.guard';
import { AuthPageComponent } from './features/auth/auth-page.component';
import { BookingPageComponent } from './features/booking/booking-page.component';
import { SearchPageComponent } from './features/search/search-page.component';

export const routes: Routes = [
	{
		path: '',
		component: SearchPageComponent,
		title: 'Search Flights | SkyRoute',
	},
	{
		path: 'auth',
		component: AuthPageComponent,
		title: 'Sign In | SkyRoute',
	},
	{
		path: 'checkout',
		component: BookingPageComponent,
		canActivate: [selectedOfferGuard, authGuard],
		title: 'Booking | SkyRoute',
	},
	{
		path: '**',
		redirectTo: '',
	},
];
