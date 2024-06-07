import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, map, throwError } from 'rxjs';
import { Static } from '../models/static';
import { DataService } from './data.service';

@Injectable({
  providedIn: 'root'
})
export class HttpService {

constructor(public http: HttpClient, public data: DataService) { }

  private api = 'https://localhost:7203/api/';


  getStatic(uuid: String): Observable<Static>{
    return this.http.get(this.api + 'Static/' + uuid)
      .pipe(
        map(response => {
          console.log("Get Static Answer");
          console.log(response);
          let currentStatic = new Static(response['id'], response['name'], response['uuid'], response['playersInfoList']);
          return currentStatic;
        }),
        catchError(error => throwError(error))
      );
  }


}


export { HttpClient };
