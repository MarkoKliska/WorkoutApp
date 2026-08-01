import { Component, inject } from '@angular/core';
import { LoaderService } from '../../../core/services/loader';

@Component({
  selector: 'app-loader',
  imports: [],
  templateUrl: './loader.html',
  styleUrl: './loader.scss',
})
export class Loader {
  private readonly loaderService = inject(LoaderService);
  protected readonly isLoading = this.loaderService.isLoading;
}