import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Loader } from './shared/ui/loader/loader';
import { ToastService } from './core/services/toast';
import { Toast } from './shared/ui/toast/toast';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    Loader,
    Toast
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly title = signal('workout-app');
}
