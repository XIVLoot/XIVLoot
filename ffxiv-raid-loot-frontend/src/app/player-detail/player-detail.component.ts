import { ChangeDetectorRef, Component, ElementRef, Inject, Input, ViewChild } from '@angular/core';
import { Player } from '../models/player';
import { Gear } from '../models/gear';
import { HttpService } from '../service/http.service'; // Importing the HttpService
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-player-detail',
  templateUrl: './player-detail.component.html',
  styleUrl: './player-detail.component.css'
})
export class PlayerDetailComponent {
  JOB : string[] = ["BlackMage", "Summoner", "RedMage", "WhiteMage", "Astrologian", "Sage",
                    "Scholar", "Ninja", "Samurai", "Reaper", "Monk", "Dragoon", "Gunbreaker", "DarkKnight",
                    "Paladin", "Warrior", "Machinist", "Bard", "Dancer", "Pictomancer", "Viper"];
  @Input({required:true}) player! : Player;
  @ViewChild('etroField') etroInputRef: ElementRef;

  constructor(public http: HttpService, private route: ActivatedRoute, public dialog: MatDialog,
              private cdr : ChangeDetectorRef,private staticEventsService: StaticEventsService
  ) { } // Constructor with dependency injection

  async onChangeGear(GearType : string, bis : boolean, event: Event){
    const selectElement = event.target as HTMLSelectElement;
    const selectedIndex = selectElement.selectedIndex;
    var NewGear : Gear;
    var GearTypeNumber : number;
    var Turn : number;
    switch (GearType){
      case "Weapon":
        Turn = 4;
        NewGear = this.player.WeaponChoice[selectedIndex];
        GearTypeNumber = 1;
        if(bis)
          this.player.bisWeaponGear = NewGear;
        else
          this.player.curWeaponGear = NewGear;
        break;
      case "Head":
        Turn = -1;
        NewGear = this.player.HeadChoice[selectedIndex];
        GearTypeNumber = 2;
        if(bis)
          this.player.bisHeadGear = NewGear;
        else
          this.player.curHeadGear = NewGear;
        break;
      case "Hands":
        Turn = -1;
        NewGear = this.player.HandsChoice[selectedIndex];
        GearTypeNumber = 4;
        if(bis)
          this.player.bisHandsGear = NewGear;
        else
          this.player.curHandsGear = NewGear;
        break;
      case "Body":
        Turn = 3;
        NewGear = this.player.BodyChoice[selectedIndex];
        GearTypeNumber = 3;
        if(bis)
          this.player.bisBodyGear = NewGear;
        else
          this.player.curBodyGear = NewGear;
        break;
      case "Legs":
        Turn = 3;
        NewGear = this.player.LegsChoice[selectedIndex];
        GearTypeNumber = 5;
        if(bis)
          this.player.bisLegsGear = NewGear;
        else
          this.player.curLegsGear = NewGear;
        break;
      case "Feet":
        Turn = -1;
        NewGear = this.player.FeetChoice[selectedIndex];
        GearTypeNumber = 6;
        if(bis)
          this.player.bisFeetGear = NewGear;
        else
          this.player.curFeetGear = NewGear;
        break;
      case "Necklace":
        Turn = 1;
        NewGear = this.player.NecklaceChoice[selectedIndex];
        GearTypeNumber = 8;
        if(bis)
          this.player.bisNecklaceGear = NewGear;
        else
          this.player.curNecklaceGear = NewGear;
        break;
      case "Earrings":
        Turn = 1;
        NewGear = this.player.EarringsChoice[selectedIndex];
        GearTypeNumber = 7;
        if(bis)
          this.player.bisEarringsGear = NewGear;
        else
          this.player.curEarringsGear = NewGear;
        break;
      case "Bracelets":
        Turn = 1;
        NewGear = this.player.BraceletsChoice[selectedIndex];
        GearTypeNumber = 9;
        if(bis)
          this.player.bisBraceletsGear = NewGear;
        else
          this.player.curBraceletsGear = NewGear;
        break;
      case "RightRing":
        Turn = 1;
        NewGear = this.player.RightRingChoice[selectedIndex];
        GearTypeNumber = 10;
        if(bis)
          this.player.bisRightRingGear = NewGear;
        else
          this.player.curRightRingGear = NewGear;
        break;
      case "LeftRing":
        Turn = 1;
        NewGear = this.player.LeftRingChoice[selectedIndex];
        if(bis)
          this.player.bisLeftRingGear = NewGear;
        else
          this.player.curLeftRingGear = NewGear;
        GearTypeNumber = 11;
        break;
    }

    //Assumed gear came from raid to give turn. Now checking and changing turn accordingly.
    if (NewGear.gearStage == "Tomes" || NewGear.gearStage == "Preparation")
      Turn = 0; // 0 Means ignore the lock. The API will not check for lock when Turn = 0.

    if (NewGear.gearStage == "Upgraded_Tomes") // If is augment then change turn to turn where the augment drops.
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

    if (Turn == -1){
      // If turn == -1 then there is ambiguity and we need to ask player if it dropped from 2 or 3 (Since head/hand/feet can drop from both)
      Turn = 0; // TODO: Ask player if it dropped from 2 or 3
    }


    await this.http.changePlayerGear(this.player.id, GearTypeNumber, NewGear.id, bis, Turn, true).subscribe((data) => {
      console.log(data);
      this.RegetPlayerInfo();
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
      console.log(data);
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
      this.RecomputePGSWholeStatic();
      this.cdr.detectChanges();
    });
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
            subMessage: "(Please reach out if this persists)",
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
            color : "green"
          }
        });
      this.dialogRef.close("Yes");
    });
  }
}
