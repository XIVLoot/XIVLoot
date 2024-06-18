import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, map, throwError } from 'rxjs';
import { Static } from '../models/static';
import { DataService } from './data.service';
import { environment } from '../../environments/environments';
@Injectable({
  providedIn: 'root'
})
export class HttpService {

constructor(public http: HttpClient, public data: DataService) { }

  private api = environment.api_url;


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

  getStaticName(uuid : string) : Observable<any>{
    const url = `${this.api}Static/GetOnlyStaticName/${uuid}`;
    return this.http.get(url, { responseType: 'text' }).pipe(
      catchError(error => throwError(() => new Error('Failed to get static name: ' + error.message)))
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
  ChangeStaticName(uuid : string, newName : string) : Observable<any>{
    const url = `${this.api}Static`; // Adjust the endpoint as necessary
    const body = {
      "name": newName,
      "uuid": uuid
    }
    return this.http.put(url, body).pipe(
      catchError(error => throwError(() => new Error('Failed to change static name: ' + error.message)))
    );
  }

  changePlayerEtro(playerId : number, newEtro : string) : Observable<any>{
    const url = `${this.api}Player/NewEtro`; // Adjust the endpoint as necessary
    const body = {
      "id": playerId,
      "UseBis": true,
      "gearToChange": 1,
      "newGearId": 0,
      "newEtro": newEtro,
      "newName": "string",
      "newJob": 0,
      "newLock": true
    }
    return this.http.put(url, body).pipe(
      map(response => 
        response as any),
        catchError(error => throwError(() => new Error('Failed to change player etro: ' + error.message)))
    );
  }

  resetPlayerJobDependantValues(playerId : number) : Observable<any>{
    const url = `${this.api}Player/ResetJobDependantValues/${playerId}`; // Adjust the endpoint as necessary

    return this.http.get(url).pipe(
      catchError(error => throwError(() => new Error('Failed to reset player job dependant values: ' + error.message)))
    );
  }

  GetSingletonPlayerInfo(uuid : string, id : number) : Observable<any>{
    const url = `${this.api}Player/GetSingletonPlayerInfo/${uuid}/${id}`;
    return this.http.get(url).pipe(
      catchError(error => throwError(() => new Error('Failed to get singleton player info: ' + error.message)))
    );
  }

  RecomputePGS(uuid : string) : Observable<any>{
    const url = `${this.api}Static/PlayerGearScore/${uuid}`;
    return this.http.get(url).pipe(
      catchError(error => throwError(() => new Error('Failed to recompute PGS: ' + error.message)))
    );
  }

  AddStatic(name : string) : Observable<any>{
    const url = `${this.api}Static/CreateNewStatic/${name}`;

    return this.http.put(url, {}, { responseType: 'text' }).pipe(
      catchError(error => throwError(() => new Error('Failed to add static: ' + error.message)))
    );
  }

  getDiscorduserInfo(accessToken: string) : Observable<any>{
    const userUrl = 'https://discord.com/api/users/@me';
    return this.http.get(userUrl, {
      headers: { 'Authorization': `Bearer ${accessToken}` }
    }).pipe(
      catchError(error => throwError(() => new Error('Failed to get discord info : ' + error.message)))
    );
  }
  SaveStaticToUser(user_discord_id : string, static_uuid : string){
    const url = `${this.api}User/AddStaticToUserSaved/${user_discord_id}/${static_uuid}`;
    return this.http.put(url, {}).pipe(
      catchError(error => throwError(() => new Error('Failed to save static to user: ' + error.message)))
    );
  }

  AddDicordUserToDB(user_discord_id : string){
    const url = `${this.api}User/AddUserDiscordId/${user_discord_id}`;
    return this.http.put(url, {}).pipe(
      catchError(error => throwError(() => new Error('Failed to add discord user to db: ' + error.message)))
    );
  }

  GetUserSavedStatic(user_discord_id : string){
    const url = `${this.api}User/GetUserSavedStatic/${user_discord_id}`;
    return this.http.get(url).pipe(
      catchError(error => throwError(() => new Error('Failed to get user saved static: ' + error.message)))
    );
  }

  RemoveUserSavedStatic(user_discord_id : string, uuid : string){
    const url = `${this.api}User/RemoveStaticToUserSaved/${user_discord_id}/${uuid}`;
    return this.http.put(url, {}, { responseType: 'text' }).pipe(
      catchError(error => throwError(() => new Error('Failed to remove user saved static: ' + error.message)))
    );
  }

  GetPGSParam(uuid : string){
    const url = `${this.api}Static/GetPGSParam/${uuid}`;
    return this.http.get(url).pipe(
      catchError(error => throwError(() => new Error('Failed to get PGS param: ' + error.message)))
    );
  }

  SetPGSParam(uuid : string, a : number, b : number, c : number){
    const url = `${this.api}Static/SetPGSParam/${uuid}/${a}/${b}/${c}`;
    return this.http.put(url, {}).pipe(
      catchError(error => throwError(() => new Error('Failed to set PGS param: ' + error.message)))
    );
  }

}


export { HttpClient };
