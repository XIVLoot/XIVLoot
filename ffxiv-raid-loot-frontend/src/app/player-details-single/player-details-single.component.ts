import { ChangeDetectorRef, Component, ElementRef, Inject, Input, ViewChild, ChangeDetectionStrategy } from '@angular/core';
import { Player } from '../models/player';
import { Gear } from '../models/gear';
import { HttpService } from '../service/http.service'; // Importing the HttpService
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-player-details-single',
  templateUrl: './player-details-single.component.html',
  styleUrl: './player-details-single.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PlayerDetailsSingleComponent {
  JOB : string[] = ["BlackMage", "Summoner", "RedMage", "WhiteMage", "Astrologian", "Sage",
                    "Scholar", "Ninja", "Samurai", "Reaper", "Monk", "Dragoon", "Gunbreaker", "DarkKnight",
                    "Paladin", "Warrior", "Machinist", "Bard", "Dancer", "Pictomancer", "Viper"];
  @Input({required:true}) player! : Player;
  @ViewChild('etroField') etroInputRef: ElementRef;

  public GetGroupColorNoAlpha : string;

  constructor(public http: HttpService, private route: ActivatedRoute, public dialog: MatDialog,
              private cdr : ChangeDetectorRef,private staticEventsService: StaticEventsService
  ) { } // Constructor with dependency injection

  ngOnInit(){
    this.player.GetGroupColorNoAlpha();
  }

  async onChangeGear(GearType : string, bis : boolean, event: Event){
    const selectElement = event.target as HTMLSelectElement;
    const selectedIndex = selectElement.selectedIndex;
    var NewGear : Gear;
    var GearTypeNumber : number;
    var Turn = 0;
    switch (GearType){
      case "Weapon":
        NewGear = this.player.WeaponChoice[selectedIndex];
        GearTypeNumber = 1;
        break;
      case "Head":
        NewGear = this.player.HeadChoice[selectedIndex];
        GearTypeNumber = 2;
        break;
      case "Hands":
        NewGear = this.player.HandsChoice[selectedIndex];
        GearTypeNumber = 4;
        break;
      case "Body":
        NewGear = this.player.BodyChoice[selectedIndex];
        GearTypeNumber = 3;
        break;
      case "Legs":
        NewGear = this.player.LegsChoice[selectedIndex];
        GearTypeNumber = 5;
        break;
      case "Feet":
        NewGear = this.player.FeetChoice[selectedIndex];
        GearTypeNumber = 6;
        break;
      case "Necklace":
        NewGear = this.player.NecklaceChoice[selectedIndex];
        GearTypeNumber = 8;
        break;
      case "Earrings":
        NewGear = this.player.EarringsChoice[selectedIndex];
        GearTypeNumber = 7;
        break;
      case "Bracelets":
        NewGear = this.player.BraceletsChoice[selectedIndex];
        GearTypeNumber = 9;
        break;
      case "RightRing":
        NewGear = this.player.RightRingChoice[selectedIndex];
        GearTypeNumber = 10;
        break;
      case "LeftRing":
        NewGear = this.player.LeftRingChoice[selectedIndex];
        GearTypeNumber = 11;
        break;
    }
    if (!bis && !(NewGear.gearStage == "Tomes" || NewGear.gearStage == "Preparation")){
      if (NewGear.gearStage == "Upgraded_Tomes"){ // If is augment then change turn to turn where the augment drops.
        switch(GearType){
          case "Weapon":
          case "Body":
          case "Head":
          case "Hands":
          case "Legs":
          case "Feet":
            Turn = 3;
            break;
          case "Necklace":
          case "Earrings":
          case "Bracelets":
          case "RightRing":
          case "LeftRing":
            Turn = 2;
            break;
        }
      }
      else{
      switch(GearType){
        case "Weapon":
          Turn = 4;
          break;
        case "Body":
        case "Legs":
          Turn = 3;
          break;
        case "Feet":
        case "Hands":
        case "Head":
          Turn = -1
          break;
        case "Necklace":
        case "Earrings":
        case "Bracelets":
        case "RightRing":
        case "LeftRing":
          Turn = 1;
            break;
        }
      }

      if (Turn == -1){
        // If turn == -1 then there is ambiguity and we need to ask player if it dropped from 2 or 3 (Since head/hand/feet can drop from both)
        Turn = await new Promise<number>((resolve) => this.dialog.open(ConfirmDialog, {
          width: '500px',
          height: '200px',
          data: {title : "Fight uncertainty", content : "The gear you selected can be dropped from turn 2 and turn 3, please select where it came from.", yes_option : "Turn 2", no_option : "Turn 3"}
        }).afterClosed().subscribe(result => {
          console.log("after closed")
          console.log(result)
          if (result === "No"){
            return resolve(3);
          } else if (result === "Yes") {
            return resolve(2);
          }
          else{
            return resolve(-1);
          }
        }));
        if (Turn === -1)
          return false;
      }
    }

    var check = await new Promise<boolean>((resolve) => {
      this.CheckConfirmGear(Turn).then((result) => {
        resolve(result);
      });
    });

    if (check){
      await this.http.changePlayerGear(this.player.id, GearTypeNumber, NewGear.id, bis, Turn).subscribe((data) => {
        console.log(data);
        this.RegetPlayerInfo();
      });
      switch (GearType){
        case "Weapon":
          if(bis)
            this.player.bisWeaponGear = NewGear;
          else
            this.player.curWeaponGear = NewGear;
          break;
        case "Head":
          if(bis)
            this.player.bisHeadGear = NewGear;
          else
            this.player.curHeadGear = NewGear;
          break;
        case "Hands":
          if(bis)
            this.player.bisHandsGear = NewGear;
          else
            this.player.curHandsGear = NewGear;
          break;
        case "Body":
          if(bis)
            this.player.bisBodyGear = NewGear;
          else
            this.player.curBodyGear = NewGear;
          break;
        case "Legs":
          if(bis)
            this.player.bisLegsGear = NewGear;
          else
            this.player.curLegsGear = NewGear;
          break;
        case "Feet":
          if(bis)
            this.player.bisFeetGear = NewGear;
          else
            this.player.curFeetGear = NewGear;
          break;
        case "Necklace":
          if(bis)
            this.player.bisNecklaceGear = NewGear;
          else
            this.player.curNecklaceGear = NewGear;
          break;
        case "Earrings":
          if(bis)
            this.player.bisEarringsGear = NewGear;
          else
            this.player.curEarringsGear = NewGear;
          break;
        case "Bracelets":
          if(bis)
            this.player.bisBraceletsGear = NewGear;
          else
            this.player.curBraceletsGear = NewGear;
          break;
        case "RightRing":
          if(bis)
            this.player.bisRightRingGear = NewGear;
          else
            this.player.curRightRingGear = NewGear;
          break;
        case "LeftRing":
          if(bis)
            this.player.bisLeftRingGear = NewGear;
          else
            this.player.curLeftRingGear = NewGear;
          break;
      }
      return true;
    }
    return false
  }

  CheckConfirmGear(turn: number): Promise<boolean> {
    return new Promise((resolve) => {
      if (this.player.IsLockedOutOfTurn(turn)) {
        this.dialog.open(ConfirmDialog, {
          width: '500px',
          height: '200px',
          data: {
            title: "Confirm choice",
            content: "The player (" + this.player.name + ") is currently locked out of gear for the turn " + turn + ". Do you still wish to proceed?",
            yes_option: "Yes",
            no_option: "No"
          }
        }).afterClosed().subscribe(result => {
          console.log("after closed");
          console.log(result);
          resolve(result === "Yes"); // Resolve the promise with true if the result is "Yes"
        });
      } else {
        resolve(true); // If not locked, resolve the promise with true immediately
      }
    });
  }

  getImageSource(gear): string {
    if (gear.gearName == "No Equipment" || (gear === null))
      return 'assets/no_gear.png';
    switch (gear.gearStage) {
      case 'Preparation':
        return 'assets/crafted_gear_icon.webp';
      case 'Tomes':
        return 'assets/tomestone_icon.png';
      case 'Raid':
        return 'assets/raid_icon.webp';
      case 'Upgraded_Tomes':
          return 'assets/tomestone_icon_upgraded.png';
    }
  }

  getBackgroundColor(){
    switch(this.player.job){
      case "BlackMage":
      case "RedMage":
      case "Summoner":
      case "Ninja":
      case "Samurai":
      case "Monk":
      case "Reaper":
      case "Dragoon":
      case "Bard":
      case "Machinist":
      case "Dancer":
      case "Viper":
      case "Pictomancer":        
        return "rgba(255, 0, 0, 0.25)";
      case "Astrologian":
      case "Sage":
      case "Scholar":
      case "WhiteMage":
        return "rgba(0,255,0,0.25)";
      case "DarkKnight":
      case "Paladin":
      case "Warrior":
      case "Gunbreaker":
        return "rgba(0, 0, 255, 0.25)";
    }
  }

  EtroDialogOpen(playerId : number){
    const newEtro = this.etroInputRef.nativeElement.value;
    this.dialog.open(EtroDialog, {
      width: '500px',
      height: '500px',
      data: {playerId, newEtro}
    }).afterClosed().subscribe(result => {
      console.log("after closed")
      console.log(result)
      if (result === "Yes"){this.RegetPlayerInfo();}
    });
  }

  onChangeName(event : Event){
    const selectElement = event.target as HTMLSelectElement;
    const value = selectElement.value;
    this.http.changePlayerName(this.player.id, value).subscribe((data) => {
      this.player.name = value;
    });
  }
  onChangeJob(event : Event){
    const selectElement = event.target as HTMLSelectElement;
    const selectedIndex = selectElement.selectedIndex+1;
    this.http.changePlayerJob(this.player.id, selectedIndex).subscribe((data) => {
    });

    // Reset Job dependant values for the player.
    this.http.resetPlayerJobDependantValues(this.player.id).subscribe((data) => {
      console.log(data);
      this.RegetPlayerInfo();
    });

    this.RegetPlayerInfo(); // Reloads and recomputes all value

  }



  RecomputePGSWholeStatic(){
      // Recomputing PGS
      this.http.RecomputePGS(this.player.staticRef.uuid).subscribe((data) => {
        console.log(data);
        var data = data["playerGearScoreList"];
        for(var i = 0; i < data.length; i++){
          var index = this.player.staticRef.players.findIndex((player, b, c) => player.id === data[i].id);
          this.player.staticRef.players[index].playerGearScore = data[i].score;
        }
        this.staticEventsService.triggerRecomputePGS(); // Triggers event to recompute recommended order.
      });
  }

  RegetPlayerInfo(){
    this.http.GetSingletonPlayerInfo(this.player.staticRef.uuid, this.player.id).subscribe((data) => {
      console.log(data);
      var newPlayer = Player.CreatePlayerFromDict(data);
      newPlayer.staticRef = this.player.staticRef;
      newPlayer.staticId = this.player.staticId;
      var index = this.player.staticRef.players.findIndex(player => player.id == this.player.id);
      this.player.staticRef.players[index] = newPlayer;
      this.player.GetGroupColorNoAlpha();
      this.RecomputePGSWholeStatic();
      this.cdr.detectChanges();
      
    });
  }

  RemoveLockOnTurn(turn : number){
    if (! this.player.IsLockedOutOfTurn(turn)){
      return;
    }
    this.dialog.open(ConfirmDialog, {
      width: '500px',
      height: '200px',
      data: {title : "Remove lock.", content : "Are you sure you want to remove the lock of this player on turn " + turn + "?", yes_option : "Yes", no_option : "No"}
    }).afterClosed().subscribe(result => {
      console.log("after closed")
      console.log(result)
      if (result === "Yes"){
        this.http.RemoveLock(this.player.id, turn).subscribe((data) => {
          console.log(data);
          this.RegetPlayerInfo();
        });
      }
    });
  }

  onMouseEnter(turn : number, event: any) {
    if (this.player.IsLockedOutOfTurn(turn)){
      event.target.style.cursor = 'pointer';
      event.target.style.filter = 'brightness(1.1)';
    }
  }
  onMouseLeave(event: any) {
    event.target.style.filter = '';
  }

}

import {
  MatDialog,
  MatDialogRef,
  MatDialogActions,
  MatDialogClose,
  MatDialogTitle,
  MatDialogContent,
  MAT_DIALOG_DATA,
} from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { DomSanitizer } from '@angular/platform-browser';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PizzaPartyAnnotatedComponent } from '../static-detail/static-detail.component';
import { catchError, of } from 'rxjs';
import { StaticEventsService } from '../service/static-events.service';

@Component({
  selector: 'import-etro',
  templateUrl: './import-etro.component.html',
  standalone: true,
  imports: [MatButtonModule, MatDialogActions, MatDialogClose, MatDialogTitle, MatDialogContent],
})
export class EtroDialog {
  constructor(public dialogRef: MatDialogRef<EtroDialog>,public http: HttpService,
    @Inject(MAT_DIALOG_DATA) public data: { playerId: number; newEtro: string, oldEtro : string },
    private sanitizer: DomSanitizer, private _snackBar: MatSnackBar
  ) {}
  public isLoading = false;
  getSafeUrl(){
  let unsafeUrl = 'https://etro.gg/embed/gearset/' + this.data.newEtro;
  return this.sanitizer.bypassSecurityTrustResourceUrl(unsafeUrl);
  }

  ImportEtro(playerId : number, newEtro : string){
    this.isLoading = true;
    this.http.changePlayerEtro(playerId, newEtro).pipe(
      catchError(err => {
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 8000,
          data: {
            message: "Error while trying to import from etro.",
            subMessage: "(Please reach out if this continues)",
            color : "red"
          }
        });
        return of(null); // Handle the error locally and return a null observable
      })
    ).subscribe((data) => {
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 3500,
          data: {
            message: "Successfuly imported gearset from etro.gg",
            subMessage: "",
            color : ""
          }
        });
      this.dialogRef.close("Yes");
    });
  }
}

@Component({
  selector: 'import-etro',
  template: `<h2 mat-dialog-title>{{data.title}}</h2>
            <mat-dialog-content>
              {{data.content}}
            </mat-dialog-content>
            <mat-dialog-actions>
              <button mat-button (click)="dialogRef.close('Yes')">{{data.yes_option}}</button>
              <button mat-button (click)="dialogRef.close('No')">{{data.no_option}}</button>
            </mat-dialog-actions>`,
  standalone: true,
  imports: [MatButtonModule, MatDialogActions, MatDialogClose, MatDialogTitle, MatDialogContent],
})
export class ConfirmDialog {
  constructor(public dialogRef: MatDialogRef<EtroDialog>,public http: HttpService,
    @Inject(MAT_DIALOG_DATA) public data: { title : string, content : string , yes_option : string, no_option : string },
    private sanitizer: DomSanitizer, private _snackBar: MatSnackBar
  ) {}

  
}
