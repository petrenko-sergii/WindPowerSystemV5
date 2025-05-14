import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { environment } from './../../environments/environment';
import { TurbineType } from './turbine-type';

@Component({
  selector: 'app-turbine-type-edit',
  templateUrl: './turbine-type-edit.component.html',
  styleUrl: './turbine-type-edit.component.scss'
})
export class TurbineTypeEditComponent implements OnInit {
  title?: string;
  form!: FormGroup;
  turbineType?: TurbineType;
  id?: number;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.form = new FormGroup({
      manufacturer: new FormControl('', Validators.required),
      model: new FormControl('', Validators.required),
      capacity: new FormControl('', [Validators.required, Validators.min(0)])
    });

    this.loadData();
  }

  loadData() {
    const idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;

    if (this.id) {
      // EDIT MODE
      const url = environment.baseUrl + 'api/turbine-types/' + this.id;
      this.http.get<TurbineType>(url).subscribe({
        next: (result) => {
          this.turbineType = result;
          this.title = "Edit - " + this.turbineType.model;
          this.form.patchValue(this.turbineType);
        },
        error: (error) => console.error(error)
      });
    } else {
      // ADD NEW MODE
      this.title = "Create a new Turbine Type";
    }
  }

  onSubmit() {
    let turbineType = this.id ? this.turbineType : <TurbineType>{};

    if (turbineType) {
      turbineType.manufacturer = this.form.controls['manufacturer'].value;
      turbineType.model = this.form.controls['model'].value;
      turbineType.capacity = +this.form.controls['capacity'].value;

      if (this.id) {
        // EDIT mode
        const url = environment.baseUrl + 'api/turbine-types/' + turbineType.id;
        this.http.put<TurbineType>(url, turbineType).subscribe({
          next: () => {
            console.log("Turbine type " + turbineType!.id + " has been updated.");
            this.router.navigate(['/turbine-types']);
          },
          error: (error) => console.error(error)
        });
      } else {
        // ADD NEW mode
        const url = environment.baseUrl + 'api/turbine-types/';
        this.http.post<TurbineType>(url, turbineType).subscribe({
          next: (result) => {
            console.log("Turbine type " + result.id + " has been created.");
            this.router.navigate(['/turbine-types']);
          },
          error: (error) => console.error(error)
        });
      }
    }
  }
}
