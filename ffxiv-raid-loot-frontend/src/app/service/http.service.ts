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

  changePlayerGear(playerId : number, GearType : number, GearId : number, useBis : boolean) : Observable<any>{
    const url = `${this.api}Player/GearToChange`; // Adjust the endpoint as necessary
    const body = {
      "id": playerId,
      "useBis": useBis,
      "gearToChange": GearType,
      "newGearId": GearId
    }
    return this.http.put(url, body).pipe(
      catchError(error => throwError(() => new Error('Failed to change player gear: ' + error.message)))
    );
  }

  changePlayerName(playerId : number, NewName : string) : Observable<any>{
    const url = `${this.api}Player/NewName`; // Adjust the endpoint as necessary
    const body = {
      "id": playerId,
      "newName": NewName
    }
    return this.http.put(url, body).pipe(
      catchError(error => throwError(() => new Error('Failed to change player name: ' + error.message)))
    );
  }

  changePlayerJob(playerId : number, NewJob : number) : Observable<any>{
    const url = `${this.api}Player/NewJob`; // Adjust the endpoint as necessary
    const body = {
      "id": playerId,
      "useBis": false,
      "gearToChange": 0,
      "newGearId": 0,
      "newEtro": "",
      "newName": "",
      "newJob": NewJob,
      "newLock": true
    }
    return this.http.put(url, body).pipe(
      catchError(error => throwError(() => new Error('Failed to change player job: ' + error.message)))
    );
  }


}


export { HttpClient };
