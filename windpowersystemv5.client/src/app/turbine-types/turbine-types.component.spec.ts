import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularMaterialModule } from '../angular-material.module';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';

import { TurbineTypesComponent } from './turbine-types.component';
import { TurbineType } from './turbine-type';
import { TurbineTypeService } from './turbine-type.service';
import { AuthService } from '../auth/auth.service';

describe('TurbineTypesComponent', () => {
  let component: TurbineTypesComponent;
  let fixture: ComponentFixture<TurbineTypesComponent>;

  beforeEach(async () => {
    let turbineTypeServiceSpy = jasmine.createSpyObj<TurbineTypeService>('TurbineTypeService', ['getData']);
    turbineTypeServiceSpy.getData.and.returnValue(
      of<TurbineType[]>([
        { id: 1, manufacturer: 'TestMan1', model: 'TestModel1', capacity: 1.5, turbineQty: 2 },
        { id: 2, manufacturer: 'TestMan2', model: 'TestModel2', capacity: 2.0, turbineQty: 3 }
      ])
    );

    // Create a mock AuthService with isAuthenticated method
    let authService = jasmine.createSpyObj<AuthService>('AuthService', ['isAuthenticated']);
    authService.isAuthenticated.and.returnValue(true);

    await TestBed.configureTestingModule({
      declarations: [TurbineTypesComponent],
      imports: [
        BrowserAnimationsModule,
        AngularMaterialModule,
        RouterTestingModule
      ],
      providers: [
        { provide: TurbineTypeService, useValue: turbineTypeServiceSpy },
        { provide: AuthService, useValue: authService }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TurbineTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display a "Turbine Types" title', () => {
    const title = fixture.nativeElement.querySelector('h1');
    expect(title.textContent).toEqual('Turbine Types');
  });

  it('should contain a table with a list of one or more turbine types', () => {
    const table = fixture.nativeElement.querySelector('table.mat-mdc-table');
    const tableRows = table.querySelectorAll('tr.mat-mdc-row');
    expect(tableRows.length).toBeGreaterThan(0);
  });
});
