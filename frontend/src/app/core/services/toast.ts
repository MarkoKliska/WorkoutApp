import { Service, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'info';

export interface ToastMessage {
  id: number;
  type: ToastType;
  message: string;
}

@Service()
export class ToastService {
  private readonly toasts = signal<ToastMessage[]>([]);
  private nextId = 0;

  readonly activeToasts = this.toasts.asReadonly();

  success(message: string, durationMs = 4000): void {
    this.show('success', message, durationMs);
  }

  error(message: string, durationMs = 5000): void {
    this.show('error', message, durationMs);
  }

  info(message: string, durationMs = 4000): void {
    this.show('info', message, durationMs);
  }

  dismiss(id: number): void {
    this.toasts.update((toasts) => toasts.filter((toast) => toast.id !== id));
  }

  private show(type: ToastType, message: string, durationMs: number): void {
    const id = ++this.nextId;
    this.toasts.update((toasts) => [...toasts, { id, type, message }]);
    setTimeout(() => this.dismiss(id), durationMs);
  }
}