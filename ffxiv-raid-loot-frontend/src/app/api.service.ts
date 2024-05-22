import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'http://localhost:5000/api/statics'; // Adjust the URL based on your API endpoint

  constructor(private http: HttpClient) { }

  getStaticGroups(): Observable<any> {
    return this.http.get(this.baseUrl);
  }
}