import { Component } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { FormControl, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

import { AiChatAnswer } from './ai-chat';
import { AiChatService } from './ai-chat.service';

@Component({
  selector: 'app-ask-ai-dialog',
  templateUrl: './ask-ai-dialog.component.html',
  styleUrl: './ask-ai-dialog.component.scss'
})
export class AskAiDialogComponent {
  public readonly maxLength = 500;

  public question = new FormControl('', [
    Validators.required,
    Validators.maxLength(this.maxLength)
  ]);

  public isLoading = false;
  public result?: AiChatAnswer;
  public errorMessage?: string;

  constructor(
    private dialogRef: MatDialogRef<AskAiDialogComponent>,
    private aiChatService: AiChatService) {
  }

  get canAsk(): boolean {
    return !this.isLoading && this.question.valid && !!this.question.value?.trim();
  }

  ask(): void {
    if (!this.canAsk) {
      return;
    }

    this.isLoading = true;
    this.result = undefined;
    this.errorMessage = undefined;

    this.aiChatService
      .askAboutTurbineTypes(this.question.value!.trim())
      .subscribe({
        next: (result) => {
          this.result = result;
          this.isLoading = false;
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessage = error.status === 401
            ? "Please log in to ask AI."
            : error.status === 400
              ? (error.error?.error ?? "The question could not be processed.")
              : "Could not get an answer. Please try again later.";
          this.isLoading = false;
        }
      });
  }

  // Closes the pop-up before navigating through a turbine type link.
  close(): void {
    this.dialogRef.close();
  }
}
