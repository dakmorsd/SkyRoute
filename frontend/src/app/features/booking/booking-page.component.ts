import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import { ClientStorageService } from '../../core/services/client-storage.service';
import { AuthActions, selectSession } from '../../store/auth.store';
import { BookingActions, selectConfirmation, selectError, selectSelectedOffer, selectStatus } from '../../store/booking.store';
import { BookingPassenger, FlightOffer } from '../../shared/models/skyroute.models';
import { DocumentRule, getDocumentRule } from '../../shared/utils/document-rules';

type PassengerGroup = FormGroup<{
  fullName: FormControl<string>;
  email: FormControl<string>;
  documentNumber: FormControl<string>;
}>;

@Component({
  selector: 'app-booking-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './booking-page.component.html',
  styleUrl: './booking-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BookingPageComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);
  private readonly formBuilder = inject(FormBuilder);
  private readonly store = inject(Store);
  private readonly router = inject(Router);
  private readonly storage = inject(ClientStorageService);

  readonly confirmation$ = this.store.select(selectConfirmation);
  readonly status$ = this.store.select(selectStatus);
  readonly error$ = this.store.select(selectError);

  selectedOffer: FlightOffer | null = null;
  documentRule: DocumentRule = getDocumentRule('Domestic');
  userEmail = '';

  readonly form = this.formBuilder.group({
    passengers: this.formBuilder.array<PassengerGroup>([]),
  });

  get passengers(): FormArray<PassengerGroup> {
    return this.form.controls.passengers;
  }

  ngOnInit(): void {
    const storedOffer = this.storage.loadSelectedOffer();
    if (storedOffer) {
      this.store.dispatch(BookingActions.hydrateSelection({ offer: storedOffer }));
    }

    this.store
      .select(selectSession)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(session => {
        this.userEmail = session?.user.email ?? '';
      });

    this.store
      .select(selectSelectedOffer)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(offer => {
        this.selectedOffer = offer;

        if (!offer) {
          this.router.navigate(['/']);
          return;
        }

        this.documentRule = getDocumentRule(offer.routeType);
        this.rebuildPassengers(offer.pricing.passengerCount);
      });
  }

  submit(): void {
    if (!this.selectedOffer) {
      this.router.navigate(['/']);
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const passengers = this.passengers.controls.map(control => control.getRawValue()) as BookingPassenger[];

    this.store.dispatch(
      BookingActions.bookingRequested({
        request: {
          offerToken: this.selectedOffer.offerToken,
          passengers,
        },
      }),
    );
  }

  startOver(): void {
    this.store.dispatch(BookingActions.clearSelection());
    this.store.dispatch(AuthActions.clearError());
    this.router.navigate(['/']);
  }

  private rebuildPassengers(passengerCount: number): void {
    if (this.passengers.length === passengerCount) {
      return;
    }

    this.passengers.clear();

    for (let index = 0; index < passengerCount; index += 1) {
      this.passengers.push(
        this.formBuilder.nonNullable.group({
          fullName: ['', [Validators.required, Validators.minLength(2)]],
          email: [index === 0 ? this.userEmail : '', [Validators.required, Validators.email]],
          documentNumber: ['', [Validators.required, Validators.pattern(this.documentRule.pattern)]],
        }),
      );
    }
  }
}