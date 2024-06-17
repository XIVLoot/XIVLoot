import { Component, OnInit, HostListener, Inject } from '@angular/core';
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

import { MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';

import {
  MatDialog,
  MatDialogRef,
  MatDialogActions,
  MatDialogClose,
  MatDialogTitle,
  MatDialogContent,
  MAT_DIALOG_DATA
} from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { DomSanitizer } from '@angular/platform-browser';
import { StaticEventsService } from '../service/static-events.service';

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
  public uuid: string; // UUID of the static
  public gridColumns = 3; // Default number of grid columns
  public groupList = [];

  constructor(public http: HttpService, private route: ActivatedRoute, private _snackBar: MatSnackBar,
    private staticEventsService: StaticEventsService
  ) {
    this.staticEventsService.recomputePGS$.subscribe(() => {
      this.groupList = this.ComputeNumberPGSGroup();
    });
   } // Constructor with dependency injection

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
      this.groupList = this.ComputeNumberPGSGroup();
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

  ComputeNumberPGSGroup(){
    let tol : number = 11;
    let PGSList = [];
    let groupList = [];
    for (let i = 0;i<this.staticDetail.players.length;i++){
      PGSList.push(this.staticDetail.players[i]);
    }
    PGSList.sort((a, b) => a.playerGearScore - b.playerGearScore);
    let minPGS = PGSList[0].playerGearScore;
    let curMinPGSIndex = 0;
    let nGroup : number = 1;
    while (true){
      let index = PGSList.findIndex((a, b, c) => a.playerGearScore - minPGS > tol); // Find first outside of tolerance
      if (index == -1){
        groupList.push([curMinPGSIndex, PGSList.length-1]);
        break;
      }
      else {
        nGroup++;
        groupList.push([curMinPGSIndex, index-1]);
        curMinPGSIndex=index;
        minPGS = PGSList[index].playerGearScore;
      }
      if (nGroup == 4){
        groupList.push([curMinPGSIndex, PGSList.length-1]);
        break;
      }
    }
    nGroup = 0;
    let playerGroupList = [];

    for (let group of groupList){
      let curGroup = [];
      for (let i = group[0];i<=group[1];i+=1){
        curGroup.push(PGSList[i]);
      }
      playerGroupList.push({group : curGroup, nGroup : nGroup});
      nGroup+=1;
    }
    for(let i = playerGroupList.length;i<4;i++){
      playerGroupList.push({group : [], nGroup : 2*i}) // 2*i so there is no color and so it is empty
    }
    return playerGroupList;
  }

  GetGroupColor(nGroup : number){
    switch(nGroup){
      case 0:
        return 'rgba(255, 247, 0, 0.3)';
      case 1:
        return 'rgba(200, 0, 255, 0.3)';
      case 2:
        return 'rgba(0, 21, 255, 0.3)';
      case 3:
        return 'rgba(38, 255, 0, 0.3)';
    }
  }

  GetGroupBorderColor(nGroup : number){
    var baseStyle = "5px solid ";
    switch(nGroup){
      case 0:
        return baseStyle + 'rgba(255, 247, 0, 0.6)';
      case 1:
        return baseStyle + 'rgba(200, 0, 255, 0.6)';
      case 2:
        return baseStyle + 'rgba(0, 21, 255, 0.6)';
      case 3:
        return baseStyle + 'rgba(38, 255, 0, 0.5)';
    }
  }

  copyToClipboard(): void {
    const valueToCopy = "http://localhost:4200/"+this.staticDetail.uuid;
    navigator.clipboard.writeText(valueToCopy)
      .then(() => {
        // Handle successful copying here, e.g., show a message
        console.log('UUID copied to clipboard!');
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 3500,
          data : {
            message : "Copied to clipboard!", 
            subMessage : "(Send the link to your friends for them to access the static)",
            color : ""
          }
        });
      })
      .catch(err => {
        // Handle errors here
        console.error('Failed to copy UUID: ', err);
      });
  }
}

@Component({
  selector: 'SnackBar',
  template: `
   <div [style.background-color]="data.color" style="width: 100%; height: 100%; border-radius:10px;padding:10px; position: relative;">
      <h2>{{ data.message }}</h2>
      <p>{{ data.subMessage }}</p>
      <button mat-button matSnackBarAction (click)="snackBarRef.dismissWithAction()" style="position: absolute; top: 10px; right: 10px;">❌</button>
    </div>

  `,
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
    text-align: center;
  }
  p {
    text-align: center; 
  }
`]
})
export class PizzaPartyAnnotatedComponent {
  constructor(public snackBarRef: MatSnackBarRef<PizzaPartyAnnotatedComponent>,
    @Inject(MAT_SNACK_BAR_DATA) public data: any,
  ) {}
}



@Component({
  selector: 'setting-PGS',
  templateUrl: './setting-PGS.component.html',
  standalone: true,
  imports: [MatButtonModule, MatDialogActions, MatDialogClose, MatDialogTitle, MatDialogContent],
})
export class SettingPGS {
  constructor(public dialogRef: MatDialogRef<SettingPGS>,public http: HttpService,
    @Inject(MAT_DIALOG_DATA) public data: { staticDetails : Static },
  ) {}

}

