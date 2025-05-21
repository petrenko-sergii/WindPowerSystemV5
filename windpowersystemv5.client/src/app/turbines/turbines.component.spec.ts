import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularMaterialModule } from '../angular-material.module';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';

import { TurbinesComponent } from './turbines.component';
import { Turbine } from './turbine';
import { TurbineService } from './turbine.service';
import { AuthService } from '../auth/auth.service';

describe('TurbinesComponent', () => {
  let component: TurbinesComponent;
  let fixture: ComponentFixture<TurbinesComponent>;
  let turbineServiceSpy: jasmine.SpyObj<TurbineService>;

  beforeEach(async () => {
    turbineServiceSpy = jasmine.createSpyObj<TurbineService>('TurbineService', ['getData']);
    turbineServiceSpy.getData.and.returnValue(
      of<Turbine[]>([
        { id: 1, serialNumber: 'SN-001', status: 'Installed', turbineTypeId: 1, manufacturer: 'Man1', model: 'Model1' },
        { id: 2, serialNumber: 'SN-002', status: 'Run', turbineTypeId: 2, manufacturer: 'Man2', model: 'Model2' }
      ])
    );

    // Create a mock AuthService with isAuthenticated method
    let authService = jasmine.createSpyObj<AuthService>('AuthService', ['isAuthenticated']);
    authService.isAuthenticated.and.returnValue(true);

    await TestBed.configureTestingModule({
      declarations: [TurbinesComponent],
      imports: [
        BrowserAnimationsModule,
        AngularMaterialModule,
        RouterTestingModule
      ],
      providers: [
        { provide: TurbineService, useValue: turbineServiceSpy },
        { provide: AuthService, useValue: authService }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TurbinesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display a "Turbines" title', () => {
    const title = fixture.nativeElement.querySelector('h1');
    expect(title.textContent).toEqual('Turbines');
  });

  it('should contain a table with a list of one or more turbines', () => {
    const table = fixture.nativeElement.querySelector('table.mat-mdc-table');
    const tableRows = table.querySelectorAll('tr.mat-mdc-row');
    expect(tableRows.length).toBeGreaterThan(0);
  });
});
