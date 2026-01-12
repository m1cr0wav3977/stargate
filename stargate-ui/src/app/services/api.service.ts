import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Person, PersonResponse } from '../models/person.model';
import { AstronautDuty, AstronautDutyResponse } from '../models/astronaut-duty.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  // Use relative path for nginx proxy in Docker, or set to 'http://localhost:5204' for local dev
  private apiUrl = '/api';

  constructor(private http: HttpClient) { }

  // Person endpoints
  getPeople(): Observable<PersonResponse> {
    return this.http.get<PersonResponse>(`${this.apiUrl}/Person`);
  }

  getPersonById(id: number): Observable<PersonResponse> {
    return this.http.get<PersonResponse>(`${this.apiUrl}/Person/${id}`);
  }

  getPersonByName(name: string): Observable<PersonResponse> {
    return this.http.get<PersonResponse>(`${this.apiUrl}/Person/name/${encodeURIComponent(name)}`);
  }

  createPerson(name: string): Observable<PersonResponse> {
    return this.http.post<PersonResponse>(`${this.apiUrl}/Person`, { name });
  }

  updatePerson(id: number, name: string): Observable<PersonResponse> {
    return this.http.put<PersonResponse>(`${this.apiUrl}/Person/${id}`, { name });
  }

  deletePerson(id: number): Observable<PersonResponse> {
    return this.http.delete<PersonResponse>(`${this.apiUrl}/Person/${id}`);
  }

  // Astronaut Duty endpoints
  getAstronautDutiesByName(name: string): Observable<AstronautDutyResponse> {
    return this.http.get<AstronautDutyResponse>(`${this.apiUrl}/AstronautDuty/name/${encodeURIComponent(name)}`);
  }

  addAstronautDuty(name: string, personId: number, rank: string, dutyTitle: string, dutyStartDate: string): Observable<AstronautDutyResponse> {
    return this.http.post<AstronautDutyResponse>(`${this.apiUrl}/AstronautDuty`, {
      name,
      personId,
      rank,
      dutyTitle,
      dutyStartDate
    });
  }

  // Astronaut Detail endpoints
  getAstronautDetailByPersonId(personId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/AstronautDetail/person/${personId}`);
  }
}

