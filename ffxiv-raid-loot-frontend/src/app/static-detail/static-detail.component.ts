import { Component, OnInit, HostListener } from '@angular/core';
import { Static } from '../models/static'; // Importing the Static model
import { HttpService } from '../service/http.service'; // Importing the HttpService
import { ActivatedRoute } from '@angular/router'; // Importing ActivatedRoute to access route parameters
import { PlayerDetailComponent } from '../player-detail/player-detail.component';

@Component({
  selector: 'app-static-detail', // Component selector used in HTML
  templateUrl: './static-detail.component.html', // HTML template for the component
  styleUrls: ['./static-detail.component.css'] // Stylesheet for the component
})
export class StaticDetailComponent implements OnInit {
  public staticDetail: Static; // Holds the details of a static
  uuid: string; // UUID of the static
  gridColumns = 3; // Default number of grid columns

  constructor(public http: HttpService, private route: ActivatedRoute) { } // Constructor with dependency injection

  ngOnInit(): void {
    // Subscribe to route parameters to get the 'uuid'
    this.route.params.subscribe(params => {
      this.uuid = params['uuid'];
    });
    console.log("Trying details");
    // Fetch static details from the server using the uuid
    this.http.getStatic(this.uuid).subscribe(details => {
      console.log("Received details");
      this.staticDetail = details; // Assign the fetched details to staticDetail
      console.log(this.staticDetail); // Log the static details to the console
    });
    this.onResize(null); // Call onResize to set initial gridColumns based on window size
  }

  onChangeStaticName(event : Event){
    const selectElement = event.target as HTMLSelectElement;
    const newValue = selectElement.value;
    this.http.ChangeStaticName(this.staticDetail.uuid, newValue).subscribe(res => {
      console.log(res);
    });
  }

  @HostListener('window:resize', ['$event']) // Listen to window resize events
  onResize(event) {
    // Adjust gridColumns based on window width
    if (window.innerWidth < 600) {
      this.gridColumns = 1;
    } else if (window.innerWidth >= 600 && window.innerWidth < 768) {
      this.gridColumns = 2;
    } else if (window.innerWidth >= 768 && window.innerWidth < 992) {
      this.gridColumns = 3;
    } else {
      this.gridColumns = 3;
    }
  }
}

