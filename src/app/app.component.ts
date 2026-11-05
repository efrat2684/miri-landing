import { Component } from '@angular/core';
import { LandingComponent } from './landing/landing.component';

@Component({
  selector: 'app-root',
  imports: [LandingComponent],
  template: '<app-landing />',
  styles: ':host { display: block; }',
})
export class AppComponent {}
