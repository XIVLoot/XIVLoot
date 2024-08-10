import { Component, Inject, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialog, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { HttpService } from '../service/http.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { ConfirmDialog } from '../player-details-single/player-details-single.component';
import { catchError, throwError } from 'rxjs';
import { PizzaPartyAnnotatedComponent } from '../static-detail/static-detail.component';
import { Player } from '../models/player';
import { CheckWeekDoneToolTip, TotalTomestonesToolTip, StartingTomeToolTip, DeleteWeekToolTip, AddWeekToolTip , AddWeekStartToolTip, CheckWeekNotDoneToolTip,
  AddGearToPlanToolTip, AddGearLockToolTip, SurplusTomestonesToolTip, TomestoneOffsetToolTip, NeededGearToolTip, EOWTomestonesToolTip,
  GearCostToolTip
} from '../tooltip';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-tome-planner',
  templateUrl: './tome-planner.component.html',
  styleUrl: './tome-planner.component.css'
})
export class TomePlannerComponent {


  public CheckWeekDoneToolTip = CheckWeekDoneToolTip;
  public TotalTomestonesToolTip = TotalTomestonesToolTip;
  public StartingTomeToolTip = StartingTomeToolTip;
  public DeleteWeekToolTip = DeleteWeekToolTip;
  public AddWeekToolTip = AddWeekToolTip;
  public AddWeekStartToolTip = AddWeekStartToolTip;
  public CheckWeekNotDoneToolTip = CheckWeekNotDoneToolTip;
  public AddGearToPlanToolTip = AddGearToPlanToolTip;
  public AddGearLockToolTip = AddGearLockToolTip;
  public SurplusTomestonesToolTip = SurplusTomestonesToolTip;
  public TomestoneOffsetToolTip = TomestoneOffsetToolTip;
  public NeededGearToolTip = NeededGearToolTip;
  public EOWTomestonesToolTip = EOWTomestonesToolTip;

  /**/public info:any = {};
  @Input() player : Player;
  public hasTomePlan : boolean = false;
  public numberWeeksDone : number = 0;
  private UpdateNeedManual : boolean = true;



  constructor(public http: HttpService,  public dialog: MatDialog,
    private _snackBar: MatSnackBar
) { } // Constructor with dependency injection

  ngOnInit(){
    this.http.GetTomePlan(this.player.id).pipe(catchError(error => {
      this.hasTomePlan = false;
      if (error.status != 404){
      this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
        duration: 8000,
        data: {
          message: "Error while getting tome plan.",
          subMessage: "Reach out if this persists.",
          color : "red"
          }
        });
      }
      return throwError(() => new Error('Failed to get tome plan : ' + error.message));
    })).subscribe(data => {
      this.info = data;
      this.numberWeeksDone = this.info.weekDone.filter(week => week).length;
      this.hasTomePlan = true;
    });
  }

  SetWeekDone(week : number, done : boolean){
    this.http.SetWeekDone(this.player.id, week, done).subscribe(data => {
      this.ngOnInit();
    });
  }

  CreateTomePlan(){
    this.http.CreateTomePlan(this.player.id).subscribe(data => {
      this.ngOnInit();
    });
  }

  AddWeekToTomePlan(week : number){
    this.http.AddWeekToTomePlan(this.player.id, week).subscribe(data => {
      this.ngOnInit();
    });
  }

  UpdateNeededTomes(){
    if (!this.UpdateNeedManual)
      return;
    

    if (typeof this.info.numberStartTomes !== 'number' || isNaN(this.info.numberStartTomes)){
      this.UpdateNeedManual = false;
      this.info.numberStartTomes = 0;
      this.UpdateNeedManual = true;
      return;
    }
    this.info.numberStartTomes = Math.floor(this.info.numberStartTomes);
    if (this.info.numberStartTomes < 0){
      this.UpdateNeedManual = false;
      this.info.numberStartTomes = 0;
      this.UpdateNeedManual = true;
    } else if (this.info.numberStartTomes > 2000){
      this.UpdateNeedManual = false;
      this.info.numberStartTomes = 2000;
      this.UpdateNeedManual = true;
    }
    this.http.SetStartTomes(this.player.id, this.info.numberStartTomes).subscribe(data => {
      this.ngOnInit();
    });
  }

  UpdateOffsetTomes(){
    if (!this.UpdateNeedManual)
      return;
    

    if (typeof this.info.numberOffsetTomes !== 'number' || isNaN(this.info.numberOffsetTomes)){
      this.UpdateNeedManual = false;
      this.info.numberOffsetTomes = 0;
      this.UpdateNeedManual = true;
      return;
    }
    this.info.numberOffsetTomes = Math.floor(this.info.numberOffsetTomes);
    if (this.info.numberOffsetTomes < 0){
      this.UpdateNeedManual = false;
      this.info.numberOffsetTomes = 0;
      this.UpdateNeedManual = true;
    } else if (this.info.numberOffsetTomes > 2000){
      this.UpdateNeedManual = false;
      this.info.numberOffsetTomes = 2000;
      this.UpdateNeedManual = true;
    }
    this.http.SetOffsetTomes(this.player.id, this.info.numberOffsetTomes).subscribe(data => {
      this.ngOnInit();
    });
  }



  RemoveFromTomePlan(week : number, gear : string){
    this.http.RemoveFromTomePlan(this.player.id, week, gear).subscribe(data => {
      this.ngOnInit();
    });
  }

  AddGearToPlan(choiceList : any, week : number){
    if (choiceList.length == 0){
      return;
    }
    this.dialog.open(AddGearDialog, 
      {
        width: '400px',
        height: '210px',
        data: {item : choiceList},
        disableClose: true
      }).afterClosed().subscribe(data =>{
        if (data !== ""){
          this.http.AddToTomePlan(this.player.id, week, data).subscribe(data => {
            this.ngOnInit();
          });
        }
      });
  }

  RemoveGearFromPlan(gear : string, week : number){
    this.http.RemoveFromTomePlan(this.player.id, week, gear).subscribe(data => {
      this.ngOnInit();
    });
  }
  RemoveWeekFromTomePlan(week : number){
    this.http.RemoveWeekFromTomePlan(this.player.id, week).subscribe(data => {
      this.ngOnInit();
    });
  }

}



@Component({
  selector: 'add-gear',
  template: `<h2 mat-dialog-title>Select gear to add to the plan.</h2>
              <mat-dialog-content>
                Only showing the gear you can afford.
                </mat-dialog-content>
              <div style="width:20%;margin-left:40%;">
              <select style="width:100%;" matNativeControl [(ngModel)]="selectedGear">
                    <option *ngFor="let choice of data.item; let i = index" [ngValue]="choice" style="background-color: {{i % 2 === 0 ? 'white' : 'rgba(0,0,0,0.1)'}};white-space: pre-line;">
                      {{selectedGearValue[choice]}}
                    </option>
              </select>
              </div>
            <mat-dialog-actions style="justify-content: space-between;border-top: 2px solid rgba(0,0,0,0.5);width:95%;margin-left:2.5%;margin-top:5px;padding-top:2px;">
              <button mat-button (click)="dialogRef.close('')">Cancel</button>
              <button mat-button (click)="this.dialogRef.close(this.selectedGear)">Add</button>
            </mat-dialog-actions>`,
  standalone: true,
  imports: [MatButtonModule, MatDialogActions, MatDialogClose, MatDialogTitle, MatDialogContent, FormsModule, CommonModule, ConfirmDialog, MatIconModule, MatTooltipModule],
})
export class AddGearDialog {

  public selectedGear : string = "";
  public selectedGearValue : any = {
    "Weapon" :     "Weapon       - 500",
    "Head" :       "Head         - 495",
    "Body" :       "Body         - 825",
    "Hands" :      "Hands        - 495",
    "Legs" :       "Legs         - 825",
    "Feet" :       "Feet         - 495",
    "Earrings" :   "Earrings     - 375",
    "Necklace" :   "Necklace     - 375",
    "Bracelets" :  "Bracelets    - 375",
    "LeftRing" :   "Left Ring    - 375",
    "RightRing" :  "Right Ring   - 375"
  };
  public GearCostToolTip = GearCostToolTip;

  constructor(public dialogRef: MatDialogRef<AddGearDialog>, public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: { item : any},
  ) {}

  CheckAdd(){
    if (this.selectedGear == ""){
      return;
    }
    if (!this.data.item.includes(this.selectedGear)){
      this.dialog.open(ConfirmDialog, {
        width: '500px',
        height: '200px',
        data: {title : "Confirm choice", content : "Adding this gear will add a week to the plan since you will otherwise lack tomes.", yes_option : "Yes", no_option : "No"}
      }).afterClosed().subscribe(
        data => {
          if (data === "Yes"){
            this.dialogRef.close(this.selectedGear);
          }
        }
      );
    } else {
      this.dialogRef.close(this.selectedGear);
    }
  }


  
}