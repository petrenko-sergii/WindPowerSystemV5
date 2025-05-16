import { Component, OnInit } from '@angular/core';
import { TurbineType } from './turbine-type';
import { TurbineTypeService } from './turbine-type.service';

@Component({
  selector: 'app-turbine-types',
  templateUrl: './turbine-types.component.html',
  styleUrl: './turbine-types.component.scss'
})
export class TurbineTypesComponent implements OnInit {
  displayedColumns: string[] = ['id', 'manufacturer', 'model', 'capacity', 'turbineQty'];
  public turbineTypes!: TurbineType[];

  constructor(
    private turbineTypeService: TurbineTypeService) {
  }

  ngOnInit(): void {
    this.loadTurbineTypes();
  }

  private loadTurbineTypes(): void {
    this.turbineTypeService.getData().subscribe({
      next: (data) => {
        this.turbineTypes = data;
      },
      error: (err) => {
        console.error('Failed to load turbine types', err);
      }
    });
  }
}
