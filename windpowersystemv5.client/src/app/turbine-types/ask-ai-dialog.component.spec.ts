import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpErrorResponse } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';

import { AngularMaterialModule } from '../angular-material.module';
import { AskAiDialogComponent } from './ask-ai-dialog.component';
import { AiChatService } from './ai-chat.service';
import { AiChatAnswer } from './ai-chat';

describe('AskAiDialogComponent', () => {
  let component: AskAiDialogComponent;
  let fixture: ComponentFixture<AskAiDialogComponent>;
  let aiChatService: jasmine.SpyObj<AiChatService>;

  const answer: AiChatAnswer = {
    answer: 'Vestas has one type.',
    turbineTypes: [
      { id: 1, manufacturer: 'Vestas', model: 'V39/600', capacity: 600, turbineQty: 2, url: '/turbine-type/1' }
    ]
  };

  beforeEach(async () => {
    aiChatService = jasmine.createSpyObj<AiChatService>('AiChatService', ['askAboutTurbineTypes']);

    await TestBed.configureTestingModule({
      declarations: [AskAiDialogComponent],
      imports: [
        BrowserAnimationsModule,
        AngularMaterialModule,
        ReactiveFormsModule,
        RouterTestingModule
      ],
      providers: [
        { provide: AiChatService, useValue: aiChatService },
        { provide: MatDialogRef, useValue: jasmine.createSpyObj('MatDialogRef', ['close']) }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AskAiDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should not ask when the question is empty', () => {
    component.question.setValue('   ');

    component.ask();

    expect(aiChatService.askAboutTurbineTypes).not.toHaveBeenCalled();
  });

  it('should show the answer and turbine type links', () => {
    aiChatService.askAboutTurbineTypes.and.returnValue(of(answer));
    component.question.setValue('Which Vestas types exist?');

    component.ask();
    fixture.detectChanges();

    expect(aiChatService.askAboutTurbineTypes).toHaveBeenCalledWith('Which Vestas types exist?');
    const element: HTMLElement = fixture.nativeElement;
    expect(element.querySelector('.answer')?.textContent).toContain('Vestas has one type.');
    const link = element.querySelector('.turbine-type-list a') as HTMLAnchorElement;
    expect(link.textContent).toContain('Vestas V39/600');
    expect(link.getAttribute('href')).toBe('/turbine-type/1');
  });

  it('should ask to log in on 401', () => {
    aiChatService.askAboutTurbineTypes.and.returnValue(
      throwError(() => new HttpErrorResponse({ status: 401 })));
    component.question.setValue('Any types?');

    component.ask();

    expect(component.errorMessage).toBe('Please log in to ask AI.');
    expect(component.isLoading).toBeFalse();
  });
});
