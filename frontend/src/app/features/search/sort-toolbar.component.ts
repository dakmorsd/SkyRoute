import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SortMode } from '../../shared/models/skyroute.models';

interface SortOption {
  value: SortMode;
  label: string;
}

@Component({
  selector: 'app-sort-toolbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sort-toolbar.component.html',
  styleUrl: './sort-toolbar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SortToolbarComponent {
  @Input() sort: SortMode = 'recommended';
  @Input() resultCount = 0;

  @Output() readonly sortChanged = new EventEmitter<SortMode>();

  readonly options: SortOption[] = [
    { value: 'recommended', label: 'Recommended' },
    { value: 'priceAsc', label: 'Lowest Price' },
    { value: 'priceDesc', label: 'Highest Price' },
    { value: 'durationAsc', label: 'Shortest Trip' },
    { value: 'departureAsc', label: 'Earliest Departure' },
  ];
}