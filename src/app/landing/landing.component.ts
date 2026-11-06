import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ContactService, ServiceId } from '../contact.service';

export interface Service {
  id: ServiceId;
  title: string;
  text: string;
}

@Component({
  selector: 'app-landing',
  imports: [ReactiveFormsModule],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss',
})
export class LandingComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly contactService = inject(ContactService);

  readonly name = 'מירי קולר';
  readonly headline = 'עריכת AI ברמה מקצועית';
  readonly lead =
    'תמונות, סרטונים, שירים ועוד — עריכה מדויקת ששומרת על הקול ועל המראה, ומוכנה לפרסום.';

  readonly services: Service[] = [
    {
      id: 'images',
      title: 'תמונות',
      text: 'ריטוש, אחידות סגנון והתאמה לקמפיין, לאתר ולרשתות — בלי לאבד את הפרטים החשובים.',
    },
    {
      id: 'videos',
      title: 'סרטונים',
      text: 'חיתוך, קצב, צבע וכתוביות לתוכן קצר וארוך, עם גימור שנראה ערוך ולא “מסונן”.',
    },
    {
      id: 'songs',
      title: 'שירים',
      text: 'ניקיון, איזון ועיבוד להקלטות ולשירים, כך שהקול יישאר טבעי והמיקס יישב במקום.',
    },
    {
      id: 'more',
      title: 'ועוד',
      text: 'סדרות תוכן, התאמה למותג ופורמטים נוספים — לפי מה שהפרויקט באמת צריך.',
    },
  ];

  readonly form = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    phone: [''],
    service: this.formBuilder.nonNullable.control<ServiceId>('images', Validators.required),
    message: ['', [Validators.required, Validators.minLength(10)]],
  });

  submitted = false;
  sending = false;
  submitError = '';

  scrollToContact(): void {
    document.getElementById('contact')?.scrollIntoView({ behavior: 'smooth' });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.sending = true;
    this.submitError = '';

    this.contactService.submit(this.form.getRawValue()).subscribe({
      next: () => {
        this.sending = false;
        this.submitted = true;
      },
      error: (error: HttpErrorResponse) => {
        this.sending = false;
        this.submitError = this.resolveSubmitError(error);
      },
    });
  }

  editAgain(): void {
    this.submitted = false;
    this.submitError = '';
  }

  invalid(controlName: 'name' | 'email' | 'phone' | 'service' | 'message'): boolean {
    const control = this.form.controls[controlName];
    return control.invalid && control.touched;
  }

  private resolveSubmitError(error: HttpErrorResponse): string {
    const body = error.error;
    const bodyText = typeof body === 'string' ? body : '';

    if (bodyText.includes('<!DOCTYPE') || bodyText.includes('<html')) {
      return 'הבקשה לא הגיעה לשרת ה-API. הפעילו: npm run start:api ואז npm start, ופתחו http://localhost:4200';
    }

    if (error.status === 0) {
      return 'לא ניתן להתחבר לשרת ב-Render. בדקו שהאתר נבנה מחדש ב-Netlify, שה-API לא «ישן» (Free tier), וש-CORS כולל netlify.app.';
    }

    if (body && typeof body === 'object' && 'message' in body && typeof body.message === 'string') {
      return body.message;
    }

    return 'שליחת הפנייה נכשלה. ודאו שהשרת פועל ונסו שוב.';
  }
}
