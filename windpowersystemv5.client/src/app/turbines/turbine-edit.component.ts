import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Turbine } from './turbine';
import { TurbineType } from './../turbine-types/turbine-type';
import { TurbineService } from './turbine.service';

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
  statuses?: string[];
  id?: number;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private turbineService: TurbineService) {
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
    this.loadStatuses();
        
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;

    if (this.id) {
      // EDIT MODE
      this.turbineService.get(this.id).subscribe({
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
    this.turbineService.getTurbineTypes().subscribe({
      next: (data) => {
        this.turbineTypes = data;
      },
      error: (error) => console.error(error)
    });
  }

  loadStatuses() {
    this.turbineService.loadStatuses().subscribe({
      next: (data) => {
        this.statuses = data;
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
        this.turbineService.put(turbine)
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
        this.turbineService.post(turbine)
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
