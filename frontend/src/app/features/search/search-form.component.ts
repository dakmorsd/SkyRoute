import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

import { Airport, CabinClass, SearchCriteria } from '../../shared/models/skyroute.models';

@Component({
  selector: 'app-search-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './search-form.component.html',
  styleUrl: './search-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SearchFormComponent implements OnChanges {
  private readonly formBuilder = inject(FormBuilder);

  @Input() airports: Airport[] = [];
  @Input() loading = false;
  @Input() submitting = false;
  @Input() initialCriteria: SearchCriteria | null = null;

  @Output() readonly searchSubmitted = new EventEmitter<SearchCriteria>();

  readonly today = new Date().toISOString().slice(0, 10);
  readonly passengerCounts = [1, 2, 3, 4, 5, 6, 7, 8, 9];
  readonly cabinClasses: CabinClass[] = ['Economy', 'Business', 'FirstClass'];

  readonly form = this.formBuilder.nonNullable.group(
    {
      originAirportCode: ['', Validators.required],
      destinationAirportCode: ['', Validators.required],
      departureDate: [this.today, Validators.required],
      passengerCount: [1, [Validators.required, Validators.min(1), Validators.max(9)]],
      cabinClass: ['Economy' as CabinClass, Validators.required],
    },
    { validators: [this.distinctAirportsValidator()] },
  );

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['initialCriteria'] && this.initialCriteria) {
      this.form.patchValue(this.initialCriteria, { emitEvent: false });
    }
  }

  get sameAirportError(): boolean {
    return Boolean(this.form.errors?.['sameAirport']) && (this.form.touched || this.form.dirty);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.searchSubmitted.emit(this.form.getRawValue());
  }

  distinctAirportsValidator(): ValidatorFn {
    return group => {
      const origin = group.get('originAirportCode')?.value;
      const destination = group.get('destinationAirportCode')?.value;

      if (origin && destination && origin === destination) {
        return { sameAirport: true } satisfies ValidationErrors;
      }

      return null;
    };
  }
}