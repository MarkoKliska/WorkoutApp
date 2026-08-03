import { Component, inject } from '@angular/core';
import { ToastService } from '../../../core/services/toast';

@Component({
  selector: 'app-toast',
  imports: [],
  templateUrl: './toast.html',
  styleUrl: './toast.scss',
})
export class Toast {
  private readonly toastService = inject(ToastService);
  protected readonly toasts = this.toastService.activeToasts;

  dismiss(id: number): void {
    this.toastService.dismiss(id);
  }
}