import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { HealthCheckComponent } from './health-check/health-check.component';
import { CitiesComponent } from './cities/cities.component';
import { CityEditComponent } from './cities/city-edit.component';
import { CountriesComponent } from './countries/countries.component';
import { CountryEditComponent } from './countries/country-edit.component';
import { TurbineTypesComponent } from './turbine-types/turbine-types.component';
import { TurbineTypeEditComponent } from './turbine-types/turbine-type-edit.component';
import { TurbinesComponent } from './turbines/turbines.component';
import { TurbineEditComponent } from './turbines/turbine-edit.component';

const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'fetch-data', component: FetchDataComponent },
  { path: 'health-check', component: HealthCheckComponent },
  { path: 'cities', component: CitiesComponent },
  { path: 'city/:id', component: CityEditComponent },
  { path: 'city', component: CityEditComponent },
  { path: 'countries', component: CountriesComponent },
  { path: 'country/:id', component: CountryEditComponent },
  { path: 'country', component: CountryEditComponent },
  { path: 'turbine-types', component: TurbineTypesComponent },
  { path: 'turbine-type/:id', component: TurbineTypeEditComponent },
  { path: 'turbine-type', component: TurbineTypeEditComponent },
  { path: 'turbines', component: TurbinesComponent },
  { path: 'turbine/:id', component: TurbineEditComponent },
  { path: 'turbine', component: TurbineEditComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
