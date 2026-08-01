import { Component, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth';

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  private readonly authService = inject(AuthService);
  protected readonly currentUser = this.authService.currentUser;
}