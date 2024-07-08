import { Component } from '@angular/core';
import { environment } from '../../environments/environments';
import { HttpService } from '../service/http.service';
import { DataService } from '../service/data.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { map } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  // Constructor with HttpClient and DataService injected
  constructor(public http: HttpService, public data: DataService, public router: Router, private _snackBar: MatSnackBar){}
  staticName: string = ''; // Property to store the name of a static

  // Lifecycle hook that is called after Angular has initialized all data-bound properties
  ngOnInit(): void {

  }
  private api = environment.api_url; // Base URL for the API
  private url = environment.site_url;

  // Asynchronous method to add a new static entity
  async AddStatic(name: string) {
    // Making a POST request to the API to add a new static
    this.http.AddStatic(name)
      .pipe(map(response => {
        // Mapping the response to a Static model
        //let newStatic = new Static(response['id'], response['name'], response['uuid'], response['players']);
        return response;
      }))
      .subscribe(response => {
        // Subscribing to the observable to handle the response
        this.data.static = response; // Storing the response in DataService
        console.log(response); // Logging the response to the console
        this.router.navigate(['/' + response]);
      });
  }

}