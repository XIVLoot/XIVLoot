import { Component, Input } from '@angular/core';
import { HttpClient, HttpService } from '../service/http.service';
import { DataService } from '../service/data.service';
import { Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { Static } from '../models/static';
import { environment } from '../../environments/environments';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PizzaPartyAnnotatedComponent } from '../static-detail/static-detail.component';

// Component decorator with metadata for StaticComponent
@Component({
  selector: 'app-static', // Selector name used in HTML to instantiate this component
  templateUrl: './static.component.html', // Path to the HTML template for this component
  styleUrl: './static.component.css' // Path to the CSS for this component
})

export class StaticComponent {
  // Constructor with HttpClient and DataService injected
  constructor(public http: HttpService, public data: DataService, public router: Router, private _snackBar: MatSnackBar){}
  staticName: string = ''; // Property to store the name of a static

  // Lifecycle hook that is called after Angular has initialized all data-bound properties
  ngOnInit(): void {

  }
  private api = environment.api_url; // Base URL for the API
  private url = environment.site_url;

  // Asynchronous method to add a new static entity
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
        //console.log(response); // Logging the response to the console
        this.router.navigate(['/' + response]);
      });
  }

  redSnackbar(){
    this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
      duration: 80000,
      data: {
        message: "Error while trying to import from etro.",
        subMessage: "(Make sure the UUID is correct)",
        color : "red"
      }
    });
  }

  greenSnackbar(){
    this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
      duration: 80000,
      data: {
        message: "Successssssssssssssssssssssssssssssssssssss",
        subMessage: "(Please celebrate)",
        color : "green"
      }
    });
  }

}
