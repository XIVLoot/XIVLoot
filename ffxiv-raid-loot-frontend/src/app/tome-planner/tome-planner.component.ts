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

  /*@Input()*/public info:any = {};
  constructor(public http: HttpService,  public dialog: MatDialog,
    private _snackBar: MatSnackBar
) { } // Constructor with dependency injection

  ngOnInit(){
    this.http.GetTomePlan(4).subscribe(data => {
      this.info = data;
    });
  }

  RemoveFromTomePlan(week : number, gear : string){
    this.http.RemoveFromTomePlan(4, week, gear).subscribe(data => {
      this.http.GetTomePlan(4).subscribe(data => {
        this.info = data;
      });
    });
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
          this.http.AddToTomePlan(4, week, data).subscribe(data => {
            this.http.GetTomePlan(4).subscribe(data => {
              this.info = data;
            });
          });
        }
      });
  }

  RemoveGearFromPlan(gear : string, week : number){
    this.http.RemoveFromTomePlan(4, week, gear).subscribe(data => {
      this.http.GetTomePlan(4).subscribe(data => {
        this.info.tomePlan = data;
      });
    });
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