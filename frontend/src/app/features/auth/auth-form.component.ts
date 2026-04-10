import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { LoginPayload, RegisterPayload } from '../../shared/models/skyroute.models';

@Component({
  selector: 'app-auth-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './auth-form.component.html',
  styleUrl: './auth-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AuthFormComponent implements OnChanges {
  private readonly formBuilder = inject(FormBuilder);

  @Input() mode: 'login' | 'register' = 'login';
  @Input() pending = false;
  @Input() error: string | null = null;

  @Output() readonly submitted = new EventEmitter<LoginPayload | RegisterPayload>();

  readonly form = this.formBuilder.nonNullable.group({
    fullName: [''],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['mode']) {
      if (this.mode === 'register') {
        this.form.controls.fullName.addValidators([Validators.required, Validators.minLength(2)]);
      } else {
        this.form.controls.fullName.clearValidators();
      }

      this.form.controls.fullName.updateValueAndValidity({ emitEvent: false });
    }
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();

    if (this.mode === 'register') {
      this.submitted.emit({
        fullName: value.fullName,
        email: value.email,
        password: value.password,
      });

      return;
    }

    this.submitted.emit({
      email: value.email,
      password: value.password,
    });
  }
}