import { Component, Input } from '@angular/core';
import { DataService } from '../service/data.service';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { Static } from '../models/static';
import { Router } from '@angular/router';

// Component decorator with metadata for StaticComponent
@Component({
  selector: 'app-static', // Selector name used in HTML to instantiate this component
  templateUrl: './static.component.html', // Path to the HTML template for this component
  styleUrl: './static.component.css' // Path to the CSS for this component
})

export class StaticComponent {
  // Constructor with HttpClient and DataService injected
  constructor(public http: HttpClient, public data: DataService, public router: Router){}
  staticName: string = ''; // Property to store the name of a static

  // Lifecycle hook that is called after Angular has initialized all data-bound properties
  ngOnInit(): void {

  }
  private api = 'https://localhost:7203/api/'; // Base URL for the API

  // Asynchronous method to add a new static entity
  async AddStatic(name: string) {
    // Making a POST request to the API to add a new static
    this.http.post(this.api + 'Static?name=' + name, {})
      .pipe(map(response => {
        // Mapping the response to a Static model
        let newStatic = new Static(response['id'], response['name'], response['uuid'], response['players']);
        return newStatic;
      }))
      .subscribe(response => {
        // Subscribing to the observable to handle the response
        this.data.static = response; // Storing the response in DataService
        console.log(response); // Logging the response to the console
        this.router.navigate(['/' + response.uuid]);
      });
  }

}
