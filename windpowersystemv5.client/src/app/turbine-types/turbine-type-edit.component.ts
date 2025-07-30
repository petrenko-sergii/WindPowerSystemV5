import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { TurbineType } from './turbine-type';
import { TurbineTypeService } from './turbine-type.service';
import * as pdfjsLib from 'pdfjs-dist/legacy/build/pdf';
import 'pdfjs-dist/build/pdf.worker.entry';

(pdfjsLib as any).GlobalWorkerOptions.workerSrc =
  // @ts-ignore
  window['pdfjsWorkerSrc'] ||
  'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/3.11.174/pdf.worker.min.js';

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
  selectedFile: File | null = null;
  selectedFileName: string = '';
  fileError: string = '';
  turbineImageUrl?: string;
  turbinePdfUrl?: string;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private turbineTypeService: TurbineTypeService
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
      this.turbineTypeService.get(this.id).subscribe({
        next: (result) => {
          this.turbineType = result;
          this.loadTurbineImage();
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

  loadTurbineImage() {
    const fileName = this.turbineType?.fileName!;

    this.turbineTypeService.getInfoFile(fileName).then(file => {
      if (file) {
        if (file.type === "image/jpeg") {
          const reader = new FileReader();
          reader.onload = () => {
            this.turbineImageUrl = reader.result as string;
          };
          reader.readAsDataURL(file);
        }
        else if (file.type === "application/pdf"){
          const fileReader = new FileReader();
          fileReader.onload = async () => {
            const typedarray = new Uint8Array(fileReader.result as ArrayBuffer);
            const pdf = await pdfjsLib.getDocument({ data: typedarray }).promise;
            const page = await pdf.getPage(1);

            const viewport = page.getViewport({ scale: 1.5 });
            const canvas = document.createElement('canvas');
            const context = canvas.getContext('2d')!;
            canvas.height = viewport.height;
            canvas.width = viewport.width;

            await page.render({ canvasContext: context, viewport }).promise;
            this.turbinePdfUrl = canvas.toDataURL();
          };
          fileReader.readAsArrayBuffer(file);
        }
      }
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      const validExtensions = ['pdf', 'jpeg', 'jpg'];
      const extension = file.name.split('.').pop()?.toLowerCase();

      if (extension && validExtensions.includes(extension)) {
        this.selectedFile = file;
        this.selectedFileName = file.name;
        this.fileError = '';
      } else {
        this.selectedFile = null;
        this.selectedFileName = '';
        this.fileError = 'Invalid file type. Only .pdf, .jpeg, .jpg allowed.';
      }
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
        this.turbineTypeService.put(turbineType).subscribe({
          next: () => {
            console.log("Turbine type " + turbineType!.id + " has been updated.");
            this.router.navigate(['/turbine-types']);
          },
          error: (error) => console.error(error)
        });
      } else {
        // ADD NEW mode
        this.turbineTypeService.createWithInfoFile(turbineType, this.selectedFile!).subscribe({
          next: (id) => {
            console.log("Turbine type " + id + " has been created.");
            this.router.navigate(['/turbine-types']);
          },
          error: (error) => console.error(error)
        });
      }
    }
  }
}
