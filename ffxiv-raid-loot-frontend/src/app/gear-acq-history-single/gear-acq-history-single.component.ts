import { ChangeDetectorRef, Component, Input } from '@angular/core';
import { Static } from '../models/static';
import { HttpService } from '../service/http.service';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialog } from '../player-details-single/player-details-single.component';
import { PizzaPartyAnnotatedComponent } from '../static-detail/static-detail.component';
import { MatSnackBar } from '@angular/material/snack-bar';


interface showInfo{
  GearType : string,
  isAugment : boolean
}

@Component({
  selector: 'app-gear-acq-history-single',
  templateUrl: './gear-acq-history-single.component.html',
  styleUrl: './gear-acq-history-single.component.css'
})
export class GearAcqHistorySingleComponent {
  @Input() week: any;
  @Input() staticRef : Static;
  public upToDate : Date;
  public isCurrent : boolean;

  public ListOfTurnInfo : any = {
    1 : {},
    2 : {},
    3 : {},
    4 : {}
  }

  public ListOfTurnInfoNumber = [];

  constructor(private http : HttpService, public dialog : MatDialog,private _snackBar : MatSnackBar){}

  ngOnInit(){


    this.upToDate = new Date(this.week.key);
    this.upToDate.setDate(this.upToDate.getDate() + 6);
    this.isCurrent = this.upToDate > new Date();

  
    for (let v of this.week.value){
      if (v.turn == 0) 
        v.turn = 1;
      var playerName = this.staticRef.players.find(p => p.id == v.playerId).name;
      if (playerName in this.ListOfTurnInfo[v.turn]){
        this.ListOfTurnInfo[v.turn][playerName].push({
          gearType : v.gearType,
          isAugment : v.isAugment,
          Id : v.id,
          IsBook : v.isAcquiredFromBook,
          turn : v.turn
        });
      } else {
        this.ListOfTurnInfo[v.turn][playerName] = [{
          gearType : v.gearType,
          isAugment : v.isAugment,
          Id : v.id,
          IsBook : v.isAcquiredFromBook,
          turn : v.turn
        }];
      }
    }
    for (let key of Object.keys(this.ListOfTurnInfo)){
      var x = Object.keys(this.ListOfTurnInfo[parseInt(key)]).length;
      this.ListOfTurnInfoNumber.push(x);
    }
  }

  getTurnImage(turn : number){
    switch(turn){
      case 1:
        switch (this.staticRef.Tier){
          case 2:
            return "assets/raid/no_image.png";
          case 1:
            return "assets/raid/no_image.png";
          case 0:
            return "assets/raid/turn_1_d.png";
        }
      case 2:
        switch (this.staticRef.Tier){
          case 2:
            return "assets/raid/no_image.png";
          case 1:
            return "assets/raid/no_image.png";
          case 0:
            return "assets/raid/turn_2_d.png";
        }
      case 3:
        switch (this.staticRef.Tier){
          case 2:
            return "assets/raid/no_image.png";
          case 1:
            return "assets/raid/no_image.png";
          case 0:
            return "assets/raid/turn_3_d.png";
        }
      case 4:
        switch (this.staticRef.Tier){
          case 2:
            return "assets/raid/no_image.png";
          case 1:
            return "assets/raid/no_image.png";
          case 0:
            return "assets/raid/turn_4_d.png";
        }
  }
}

  GetIcon(v){
    if (v.isAugment){
      switch(v.gearType){
        case "Weapon":
        case "Body":
        case "Head":
        case "Hands":
        case "Legs":
        case "Feet":
          return (v.IsBook ? "assets/twine_icon_book.png" : "assets/twine_icon.png");
        default:
          return (v.IsBook ? "assets/shine_icon_book.png" : "assets/shine_icon.png");
      }
    }
    return (v.IsBook ? "assets/gear_icon/"+v.gearType+"_chest_book_"+v.turn+".png" : "assets/gear_icon/"+v.gearType+"_chest.png");
  }

  mouseOver(event : any){
    event.target.style.outline = "2px solid rgba(255, 255, 255, 0.7)";
    event.target.style.cursor = 'pointer';
  }
  mouseLeave(event : any){
    event.target.style.outline = "";
  }

  async DeleteGearAcqEvent(turn : number, gearAcq : any, playerName : string, id : number){
    var check = await new Promise((resolve) => {
        this.dialog.open(ConfirmDialog, {
          width: '500px',
          height: '200px',
          data: {
            title: "Confirm choice",
            content: "Are you sure you want to delete this gear acquisition event? ("+gearAcq.gearType+", "+(gearAcq.isAugment ? "Augment, " : "")+playerName+", turn "+turn+")",
            subContent : "THIS ACTION IS IRREVERSIBLE.",
            yes_option: "Yes",
            no_option: "No"
          }
        }).afterClosed().subscribe(result => {
          ////console.log("after closed");
          ////console.log(result);
          resolve(result === "Yes"); // Resolve the promise with true if the result is "Yes"
        });
    });
    if (check){
      this.ListOfTurnInfo[turn][playerName] = this.ListOfTurnInfo[turn][playerName].filter(x => x.Id != id);
      if (this.ListOfTurnInfo[turn][playerName].length == 0){
        delete this.ListOfTurnInfo[turn][playerName];
        this.ListOfTurnInfoNumber[turn-1]-=1;
      }
      this.http.DeleteGearAcqEvent(id).subscribe(data => {
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 3500,
          data : {
            message : "Successfully removed history.", 
            subMessage : "",
            color : "Green"
          }
        });
      });
    }
  }

}
