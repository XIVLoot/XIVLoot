import { Component, Inject, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialog, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { HttpService } from '../service/http.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-tome-planner',
  templateUrl: './tome-planner.component.html',
  styleUrl: './tome-planner.component.css'
})
export class TomePlannerComponent {

  /*@Input()*/public info:any;
  constructor(public http: HttpService,  public dialog: MatDialog,
    private _snackBar: MatSnackBar
) { } // Constructor with dependency injection

  ngOnInit(){
    this.info = {
      "neededTomes" : 2000,
      "numberWeeks": 0,
      "numberStartTomes": 0,
      "numberOffsetTomes": 0,
      "gearPlanOrder": [
        {
          "gearName": [],
          "tomeAmountByEOW": 75,
          "tomeLeeWayAmount": 450,
          "futureTomeNeed": 0,
          "surplusTome": 0,
          "costOfWeek": 375,
          "optionList": []
        },
        {
          "gearName": ["Earrings","Earrings"],
          "tomeAmountByEOW": 150,
          "tomeLeeWayAmount": 400,
          "futureTomeNeed": 50,
          "surplusTome": 450,
          "costOfWeek": 375,
          "optionList": [
            "Earrings",
            "Necklace",
            "Bracelets",
            "LeftRing",
            "RightRing"
          ]
        },
        {
          "gearName": ["Weapon"],
          "tomeAmountByEOW": 100,
          "tomeLeeWayAmount": 450,
          "futureTomeNeed": 0,
          "surplusTome": 850,
          "costOfWeek": 500,
          "optionList": [
            "Weapon",
            "Head",
            "Hands",
            "Feet",
            "Earrings",
            "Necklace",
            "Bracelets",
            "LeftRing",
            "RightRing"
          ]
        },
        {
          "gearName": ["Empty"],
          "tomeAmountByEOW": 550,
          "tomeLeeWayAmount": 30,
          "futureTomeNeed": 420,
          "surplusTome": 1300,
          "costOfWeek": 0,
          "optionList": [
            "Body",
            "Legs",
            "Weapon",
            "Head",
            "Hands",
            "Feet",
            "Earrings",
            "Necklace",
            "Bracelets",
            "LeftRing",
            "RightRing"
          ]
        },
        {
          "gearName": ["Head"],
          "tomeAmountByEOW": 505,
          "tomeLeeWayAmount": 75,
          "futureTomeNeed": 375,
          "surplusTome": 1330,
          "costOfWeek": 495,
          "optionList": [
            "Body",
            "Legs",
            "Weapon",
            "Head",
            "Hands",
            "Feet",
            "Earrings",
            "Necklace",
            "Bracelets",
            "LeftRing",
            "RightRing"
          ]
        },
        {
          "gearName": ["Legs"],
          "tomeAmountByEOW": 130,
          "tomeLeeWayAmount": 450,
          "futureTomeNeed": 0,
          "surplusTome": 1405,
          "costOfWeek": 825,
          "optionList": [
            "Body",
            "Legs",
            "Weapon",
            "Head",
            "Hands",
            "Feet",
            "Earrings",
            "Necklace",
            "Bracelets",
            "LeftRing",
            "RightRing"
          ]
        }
      ]
    };
  }
  
  AddGearToPlan(choiceList : any, week : number){
    if (choiceList.length == 0){
      return;
    }
    this.dialog.open(AddGearDialog, 
      {
        width: '400px',
        height: '150px',
        data: {item : choiceList},
        disableClose: true
      }).afterClosed().subscribe(data =>{
        if (data !== ""){
          this.info.gearPlanOrder[week].gearName.push(data);
        }
      });
  }

  RemoveGearFromPlan(index : number, week : number){
    this.info.gearPlanOrder[week].gearName.splice(index, 1);
  }

}

@Component({
  selector: 'add-gear',
  template: `<h2 mat-dialog-title>Select gear to add</h2>
              <div style="width:95%;margin-left:2.5%;">
                  <select style="width:100%;" matNativeControl [(ngModel)]="selectedGear">
                    <option *ngFor="let choice of data.item">{{choice}}</option>
                  </select>
              </div>
            <mat-dialog-actions>
              <button mat-button (click)="dialogRef.close('')">Cancel</button>
              <button mat-button (click)="dialogRef.close(selectedGear)">Add</button>
            </mat-dialog-actions>`,
  standalone: true,
  imports: [MatButtonModule, MatDialogActions, MatDialogClose, MatDialogTitle, MatDialogContent, FormsModule, CommonModule],
})
export class AddGearDialog {

  public selectedGear : string = "";

  constructor(public dialogRef: MatDialogRef<AddGearDialog>,
    @Inject(MAT_DIALOG_DATA) public data: { item : any},
  ) {}


  
}