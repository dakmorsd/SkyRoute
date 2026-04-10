import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';
import { filter } from 'rxjs';

import { AuthSession, LoginPayload, RegisterPayload } from '../../shared/models/skyroute.models';
import { AuthActions, selectError, selectSession, selectStatus } from '../../store/auth.store';
import { AuthFormComponent } from './auth-form.component';

@Component({
  selector: 'app-auth-page',
  standalone: true,
  imports: [CommonModule, RouterLink, AuthFormComponent],
  templateUrl: './auth-page.component.html',
  styleUrl: './auth-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AuthPageComponent implements OnInit {
  private readonly store = inject(Store);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly status$ = this.store.select(selectStatus);
  readonly error$ = this.store.select(selectError);

  mode: 'login' | 'register' = 'login';
  redirectUrl = '/';

  ngOnInit(): void {
    this.mode = this.route.snapshot.queryParamMap.get('mode') === 'register' ? 'register' : 'login';
    this.redirectUrl = this.route.snapshot.queryParamMap.get('redirect') ?? '/';
    this.store.dispatch(AuthActions.clearError());

    this.store
      .select(selectSession)
      .pipe(
        filter((session): session is AuthSession => session !== null),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        this.router.navigateByUrl(this.redirectUrl);
      });
  }

  setMode(mode: 'login' | 'register'): void {
    this.mode = mode;
    this.store.dispatch(AuthActions.clearError());
  }

  submit(payload: LoginPayload | RegisterPayload): void {
    if (this.mode === 'register') {
      this.store.dispatch(AuthActions.registerRequested({ registration: payload as RegisterPayload }));
      return;
    }

    this.store.dispatch(AuthActions.loginRequested({ credentials: payload as LoginPayload }));
  }
}