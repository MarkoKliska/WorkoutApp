import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../services/user';
import { AuthService } from '../../../core/services/auth';
import { ToastService } from '../../../core/services/toast';
import { RouteNames } from '../../../shared/consts/routes';

@Component({
  selector: 'app-profile-page',
  imports: [ReactiveFormsModule],
  templateUrl: './profile-page.html',
  styleUrl: './profile-page.scss',
})
export class ProfilePage {
  private readonly fb = inject(FormBuilder);
  private readonly userService = inject(UserService);
  private readonly authService = inject(AuthService);
  private readonly toastService = inject(ToastService);
  private readonly router = inject(Router);

  protected readonly isSavingProfile = signal(false);
  protected readonly isDeletingAccount = signal(false);

  protected readonly profileForm = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email]],
  });

  protected readonly deleteForm = this.fb.nonNullable.group({
    password: ['', Validators.required],
  });

  constructor() {
    this.userService.getProfile().subscribe((profile) => {
      this.profileForm.setValue({
        firstName: profile.firstName,
        lastName: profile.lastName,
        email: profile.email,
      });
    });
  }

  saveProfile(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.isSavingProfile.set(true);

    this.userService.updateProfile(this.profileForm.getRawValue()).subscribe({
      next: () => {
        this.isSavingProfile.set(false);
        this.toastService.success('Profile updated.');
      },
      error: (error) => {
        this.isSavingProfile.set(false);
        this.toastService.error(error.error?.detail ?? 'Could not update your profile.');
      },
    });
  }

  deleteAccount(): void {
    if (this.deleteForm.invalid) {
      this.deleteForm.markAllAsTouched();
      return;
    }

    this.isDeletingAccount.set(true);

    this.userService.deleteAccount(this.deleteForm.getRawValue()).subscribe({
      next: () => {
        this.authService.logout();
        this.router.navigate(['/', RouteNames.Login]);
      },
      error: (error) => {
        this.isDeletingAccount.set(false);
        this.toastService.error(error.error?.detail ?? 'Could not delete your account.');
      },
    });
  }
}