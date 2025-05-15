import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Turbine } from './turbine';

@Component({
  selector: 'app-turbines',
  templateUrl: './turbines.component.html',
  styleUrl: './turbines.component.scss'
})
export class TurbinesComponent implements OnInit {
  displayedColumns: string[] = ['id', 'serialNumber', 'status', 'typeInfo'];
  public turbines!: Turbine[];
  private readonly apiUrl = 'api/Turbines';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadTurbines();
  }

  private loadTurbines(): void {
    this.http.get<Turbine[]>(this.apiUrl).subscribe({
      next: (data) => {
        this.turbines = data;
      },
      error: (err) => {
        console.error('Failed to load turbines', err);
      }
    });
  }
}
