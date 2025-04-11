import { Component } from '@angular/core';
import { map } from 'rxjs';
import { HttpService } from '../service/http.service';
import { DataService } from '../service/data.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-static',
  templateUrl: './create-static.component.html',
  styleUrl: './create-static.component.css'
})
export class CreateStaticComponent {
  // Asynchronous method to add a new static entity
  constructor(public http: HttpService, public data: DataService, public router: Router){}
  public staticName : string = "";
  public Tier : number = 1;
  async AddStatic(name: string, Tier : number) {
    // Making a POST request to the API to add a new static
    this.http.AddStatic(name, Tier)
      .pipe(map(response => {
        // Mapping the response to a Static model
        //let newStatic = new Static(response['id'], response['name'], response['uuid'], response['players']);
        return response;
      }))
      .subscribe(response => {
        // Subscribing to the observable to handle the response
        this.data.static = response; // Storing the response in DataService
        ////console.log(response); // Logging the response to the console
        this.router.navigate(['/' + response]);
      });
  }
}
