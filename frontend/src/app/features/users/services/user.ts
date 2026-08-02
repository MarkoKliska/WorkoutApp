import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { UpdateProfileRequest } from '../../../shared/dto/requests/users/update-profile-request';
import { DeleteAccountRequest } from '../../../shared/dto/requests/users/delete-account-request';
import { ProfileResponse } from '../../../shared/dto/requests/users/profile-response';

@Service()
export class UserService {
  private readonly http = inject(HttpClient);

  getProfile(): Observable<ProfileResponse> {
    return this.http.get<ProfileResponse>(`${environment.apiUrl}/users/me`);
  }

  updateProfile(request: UpdateProfileRequest): Observable<ProfileResponse> {
    return this.http.put<ProfileResponse>(`${environment.apiUrl}/users/me`, request);
  }

  deleteAccount(request: DeleteAccountRequest): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/users/me`, { body: request });
  }
}