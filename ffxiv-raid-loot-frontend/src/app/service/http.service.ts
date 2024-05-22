import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, map } from 'rxjs';
import { Static } from '../models/static';

@Injectable({
  providedIn: 'root'
})
export class HttpService {

constructor(public http: HttpClient) { }

  private api = 'https://localhost:7203/api/';

  addStatic(staticName: string) {
    console.log('https://localhost:7203/api/static?name=' + staticName);

    return this.http.post<any>('https://localhost:7203/api/static?name=test02', {})
    .pipe(map(response => {
      console.log(response);
      //let newStatic = new Static(response.id, response.name, response.uuid, response.players);
    }));
  }

}
