import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

export type ServiceId = 'images' | 'videos' | 'songs' | 'more';

export interface ContactPayload {
  name: string;
  email: string;
  phone: string;
  service: ServiceId;
  message: string;
}

export interface ContactResponse {
  message: string;
}

@Injectable({ providedIn: 'root' })
export class ContactService {
  private readonly http = inject(HttpClient);

  submit(payload: ContactPayload): Observable<ContactResponse> {
    return this.http.post<ContactResponse>(`${environment.apiUrl}/contact`, {
      name: payload.name,
      email: payload.email,
      phone: payload.phone || null,
      service: payload.service,
      message: payload.message,
    });
  }
}
