import { Component, OnInit } from '@angular/core';
import { Turbine } from './turbine';
import { TurbineService } from './turbine.service';

@Component({
  selector: 'app-turbines',
  templateUrl: './turbines.component.html',
  styleUrl: './turbines.component.scss'
})
export class TurbinesComponent implements OnInit {
  displayedColumns: string[] = ['id', 'serialNumber', 'status', 'typeInfo'];
  public turbines!: Turbine[];

  constructor(private turbineService: TurbineService) {}

  ngOnInit(): void {
    this.loadTurbines();
  }

  private loadTurbines(): void {
    this.turbineService.getData().subscribe({
      next: (data) => {
        this.turbines = data;
      },
      error: (err) => {
        console.error('Failed to load turbines', err);
      }
    });
  }
}
