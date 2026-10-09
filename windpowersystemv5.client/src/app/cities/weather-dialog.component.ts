import { Component, Inject, OnInit } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { City } from './city';
import { WeatherClothing } from './weather';
import { WeatherService } from './weather.service';

@Component({
  selector: 'app-weather-dialog',
  templateUrl: './weather-dialog.component.html',
  styleUrls: ['./weather-dialog.component.scss']
})
export class WeatherDialogComponent implements OnInit {
  public result?: WeatherClothing;
  public errorMessage?: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) public city: City,
    private weatherService: WeatherService) {
  }

  ngOnInit() {
    this.weatherService
      .getClothingAdvice(this.city.name, this.city.lat, this.city.lon)
      .subscribe({
        next: (result) => this.result = result,
        error: (error: HttpErrorResponse) => {
          this.errorMessage = error.status === 401
            ? "Please log in to check the weather."
            : "Could not load the weather. Please try again later.";
        }
      });
  }

  // e.g. "+24°C", "-3°C"
  formatTemperature(value: number): string {
    var rounded = Math.round(value);
    return (rounded > 0 ? "+" : "") + rounded + "°C";
  }
}
