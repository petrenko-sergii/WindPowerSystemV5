import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable, Subject } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';

import { City } from './city';
import { Country } from './../countries/country';
import { BaseFormComponent } from '../base-form.component';
import { CityService } from './city.service';

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss']
})

export class CityEditComponent
  extends BaseFormComponent implements OnInit, OnDestroy {
  title?: string;
  city?: City;

  // the countries observable for the select (using async pipe)
  countries?: Observable<Country[]>;

  // the city object id, as fetched from the active route:
  // It's NULL when we're adding a new city,
  // and not NULL when we're editing an existing one.
  id?: number;

  // Activity Log (for debugging purposes)
  activityLog: string = '';

  private destroySubject = new Subject();

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private cityService: CityService) {
    super();
  }

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl('', Validators.required),
      lat: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)
      ]),
      lon: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)
      ]),
      countryId: new FormControl('', Validators.required)
    }, null, this.isDupeCity());

    // react to form changes (for debugging purposes)
    this.form.valueChanges
      .pipe(takeUntil(this.destroySubject))
      .subscribe(() => {
        if (!this.form.dirty) {
          this.log("Form Model has been loaded.");
        }
        else {
          this.log("Form was updated by the user.");
        }
      });

    this.form.get("name")!.valueChanges
      .pipe(takeUntil(this.destroySubject))
      .subscribe(() => {
        if (!this.form.dirty) {
          this.log("Name has been loaded with initial values.");
        }
        else {
          this.log("Name was updated by the user.");
        }
      });

    this.loadData();
  }

  log(str: string) {
    const logMessage = "[" + new Date().toLocaleString() + "] " + str;

    this.activityLog += logMessage + "<br />";
    console.log(logMessage);
  }

  loadData() {
    this.loadCountries();

    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;

    if (this.id) {
      // EDIT MODE
      this.cityService.get(this.id).subscribe({
        next: (result) => {
          this.city = result;
          this.title = "Edit - " + this.city.name;
          this.form.patchValue(this.city);
        },
        error: (error) => console.error(error)
      });
    }
    else {
      // ADD NEW MODE
      this.title = "Create a new City";
    }
  }

  loadCountries() {
    let pageIndex = 0;
    let pageSize = 9999;

    this.countries = this.cityService
      .getCountries(
        pageIndex,
        pageSize,
        "name",
        "asc",
        null,
        null)
      .pipe(map(x => x.data));
  }

  onSubmit() {
    var city = (this.id) ? this.city : <City>{};

    if (city) {
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;
      city.countryId = +this.form.controls['countryId'].value;

      if (this.id) {
        // EDIT mode
        this.cityService
          .put(city)
          .subscribe({
            next: (result) => {
              console.log("City " + city!.id + " has been updated.");
              this.router.navigate(['/cities']);
            },
            error: (error) => console.error(error)
          });
      }
      else {
        // ADD NEW mode
        this.cityService
          .post(city)
          .subscribe({
            next: (result) => {
              console.log("City " + result.id + " has been created.");
              this.router.navigate(['/cities']);
            },
            error: (error) => console.error(error)
          });
      }
    }
  }

  isDupeCity(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
      var city = <City>{};
      city.id = (this.id) ? this.id : 0;
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;
      city.countryId = +this.form.controls['countryId'].value;

      return this.cityService.isDupeCity(city)
        .pipe(map(result => {
          return (result ? { isDupeCity: true } : null);
      }));
    }
  }

  ngOnDestroy() {
    // emit a value with the takeUntil notifier
    this.destroySubject.next(true);
    // complete the subject
    this.destroySubject.complete();
  }
}
