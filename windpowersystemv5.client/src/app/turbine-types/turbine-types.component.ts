import { Component, OnInit } from '@angular/core';
import { TurbineType } from './turbine-type';
import { TurbineTypeService } from './turbine-type.service';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-turbine-types',
  templateUrl: './turbine-types.component.html',
  styleUrl: './turbine-types.component.scss'
})
export class TurbineTypesComponent implements OnInit {
  displayedColumns: string[] = ['id', 'manufacturer', 'model', 'capacity', 'turbineQty', 'infoFile'];
  public turbineTypes!: TurbineType[];

  constructor(
    private turbineTypeService: TurbineTypeService,
    private authService: AuthService) {
  }

  ngOnInit(): void {
    this.loadTurbineTypes();
  }

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  downloadInfoFile(infoFileName: string) {
    this.turbineTypeService.downloadInfoFile(infoFileName);
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
