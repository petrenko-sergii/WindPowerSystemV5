import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TurbineType } from './turbine-type';

@Component({
  selector: 'app-turbine-types',
  templateUrl: './turbine-types.component.html',
  styleUrl: './turbine-types.component.scss'
})
export class TurbineTypesComponent implements OnInit {
  displayedColumns: string[] = ['id', 'manufacturer', 'model', 'capacity'];
  public turbineTypes!: TurbineType[];

  private readonly apiUrl = 'api/turbine-types'; // Base URL for the API

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadTurbineTypes();
  }

  private loadTurbineTypes(): void {
    this.http.get<TurbineType[]>(this.apiUrl).subscribe({
      next: (data) => {
        this.turbineTypes = data;
      },
      error: (err) => {
        console.error('Failed to load turbine types', err);
      }
    });
  }
}
