import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { environment } from './../../environments/environment';
import { Turbine } from './turbine';
import { TurbineType } from './../turbine-types/turbine-type';

@Component({
  selector: 'app-turbine-edit',
  templateUrl: './turbine-edit.component.html',
  styleUrls: ['./turbine-edit.component.scss']
})
export class TurbineEditComponent implements OnInit {
  title?: string;
  form!: FormGroup;
  turbine?: Turbine;
  turbineTypes?: TurbineType[];
  id?: number;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient) {
  }

  ngOnInit() {
    this.form = new FormGroup({
      serialNumber: new FormControl('', Validators.required),
      status: new FormControl('', Validators.required),
      turbineTypeId: new FormControl('', Validators.required)
    });

    this.loadData();
  }

  loadData() {
    this.loadTurbineTypes();

    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;

    if (this.id) {
      // EDIT MODE
      var url = environment.baseUrl + 'api/Turbines/' + this.id;
      this.http.get<Turbine>(url).subscribe({
        next: (result) => {
          this.turbine = result;
          this.title = "Edit turbine: " + this.turbine.serialNumber;
          this.form.patchValue(this.turbine);
        },
        error: (error) => console.error(error)
      });
    }
    else {
      // ADD NEW MODE
      this.title = "Create a new Turbine";
    }
  }

  loadTurbineTypes() {
    var url = environment.baseUrl + 'api/turbine-types';

    this.http.get<TurbineType[]>(url).subscribe({
      next: (data) => {
        this.turbineTypes = data;
      },
      error: (error) => console.error(error)
    });
  }

  onSubmit() {
    var turbine = (this.id) ? this.turbine : <Turbine>{};

    if (turbine) {
      turbine.serialNumber = this.form.controls['serialNumber'].value;
      turbine.status = this.form.controls['status'].value;
      turbine.turbineTypeId = +this.form.controls['turbineTypeId'].value;

      if (this.id) {
        // EDIT mode
        var url = environment.baseUrl + 'api/Turbines/' + turbine.id;
        this.http
          .put<Turbine>(url, turbine)
          .subscribe({
            next: (result) => {
              console.log("Turbine " + turbine!.id + " has been updated.");
              this.router.navigate(['/turbines']);
            },
            error: (error) => console.error(error)
          });
      }
      else {
        // ADD NEW mode
        var url = environment.baseUrl + 'api/Turbines';
        this.http
          .post<Turbine>(url, turbine)
          .subscribe({
            next: (result) => {
              console.log("Turbine " + result.id + " has been created.");
              this.router.navigate(['/turbines']);
            },
            error: (error) => console.error(error)
          });
      }
    }
  }
}
