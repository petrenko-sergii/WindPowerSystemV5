import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { City } from './city';

@Component({
  selector: 'app-weather-dialog',
  templateUrl: './weather-dialog.component.html'
})
export class WeatherDialogComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public city: City) {
  }
}
