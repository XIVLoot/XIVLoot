import { Component, OnInit, HostListener } from '@angular/core';
import { Static } from '../models/static'; // Importing the Static model
import { HttpService } from '../service/http.service'; // Importing the HttpService
import { ActivatedRoute } from '@angular/router'; // Importing ActivatedRoute to access route parameters
import { PlayerDetailComponent } from '../player-detail/player-detail.component';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {
  MatSnackBar,
  MatSnackBarAction,
  MatSnackBarActions,
  MatSnackBarLabel,
  MatSnackBarRef,
} from '@angular/material/snack-bar';
interface PlayerPGS {
  name: string;
  job: string;
  PGS: number;
}

@Component({
  selector: 'app-static-detail', // Component selector used in HTML
  templateUrl: './static-detail.component.html', // HTML template for the component
  styleUrls: ['./static-detail.component.css'], // Stylesheet for the component
})
export class StaticDetailComponent implements OnInit {
  public staticDetail: Static; // Holds the details of a static
  uuid: string; // UUID of the static
  gridColumns = 3; // Default number of grid columns
  PGSList : PlayerPGS[] = [];

  constructor(public http: HttpService, private route: ActivatedRoute, private _snackBar: MatSnackBar) { } // Constructor with dependency injection

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
      this.PGSList = this.ComputePGSPriority();
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

  ComputePGSPriority() : PlayerPGS[]{
    let PGSList = [];
    if (this.staticDetail.players.length === 0){
      return [{name : "No players", job : "", PGS : 0}];
    }
    for (let i = 0;i<this.staticDetail.players.length;i++){
      PGSList.push(this.staticDetail.players[i]);
    }
    PGSList.sort((a, b) => a.playerGearScore - b.playerGearScore);
    let rList : PlayerPGS[] = [];
    let tol : number = 2;
    let minPGS = PGSList[0].playerGearScore;
    for(var i = 0; i < PGSList.length; i++){
      if (PGSList[i].playerGearScore - minPGS < tol){
        rList.push(PGSList[i]);
      }
    }
    return rList;
  }
  GetPGSPriorityInfo(){
    let l = this.ComputePGSPriority();
    let r = "";
    for (let i = 0;i<l.length;i++){
      r += l[i].name + ", ";
    }
    return r;
  }

  copyToClipboard(): void {
    const valueToCopy = "http://localhost:4200/"+this.staticDetail.uuid; // Assuming you want to copy the UUID
    navigator.clipboard.writeText(valueToCopy)
      .then(() => {
        // Handle successful copying here, e.g., show a message
        console.log('UUID copied to clipboard!');
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 3.5 * 1000,
        });
      })
      .catch(err => {
        // Handle errors here
        console.error('Failed to copy UUID: ', err);
      });
  }
}

@Component({
  selector: 'snack-bar-annotated-component-example-snack',
  template: '<h2>Copied to clipboard!</h2><br><p>(Send the link to your friends for them to access the static)</p>',
  styles: [`
  :host {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    height: 100%; 
  }
  h2 {
    color: black;
    margin-bottom: 10px; 
  }
  p {
    text-align: center; 
  }
`]
})
export class PizzaPartyAnnotatedComponent {
}

