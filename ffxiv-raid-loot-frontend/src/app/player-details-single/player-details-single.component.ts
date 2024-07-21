import { ChangeDetectorRef, Component, ElementRef, Inject, Input, ViewChild, ChangeDetectionStrategy } from '@angular/core';
import { Player } from '../models/player';
import { Gear } from '../models/gear';
import { HttpClient, HttpService } from '../service/http.service'; // Importing the HttpService
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

  public oldJob : string = "";

  public etroToolTip = etroToolTip;
  public pgsOnPlayerToolTip = pgsOnPlayerToolTip;
  public lockOnPlayerToolTip = lockOnPlayerToolTip;
  public gearSelectionToolTip = gearSelectionToolTip;

  public BisIsEtro : boolean = false;
  public BisIsXIVGear : boolean = false;

  constructor(public http: HttpService, private route: ActivatedRoute, public dialog: MatDialog,
              private cdr : ChangeDetectorRef,private staticEventsService: StaticEventsService,
              private _snackBar: MatSnackBar
  ) { } // Constructor with dependency injection

  ngOnInit(){
    this.player.GetGroupColorNoAlpha();
    this.oldJob = this.player.job;

    if (this.player.etroBiS.includes("xivgear.app"))
      this.BisIsXIVGear = true;
    else if (this.player.etroBiS.includes("etro.gg"))
      this.BisIsEtro = true;
  }

  unauthorized(){
    this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
      duration: 3500,
      data: {
        message: "You have not claimed this player. Only its owner can modify it.",
        subMessage: "The changes will be reverted",
        color : "red"
      }
    });
  }

  async onChangeGear(GearType : string, bis : boolean, event: Event){
    const selectElement = event.target as HTMLSelectElement;
    const selectedIndex = selectElement.selectedIndex;
    var oldGear;
    if (selectedIndex == 0){
      // Reverting change
      this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
        duration: 3500,
        data: {
          message: "You cannot select the 'NoEquipment' option.",
          subMessage: "",
          color : "red"
        }
      });
      switch (GearType){
        case "Weapon":
          if(bis)
            selectElement.value = this.player.bisWeaponGear.gearName;
          else
            selectElement.value = this.player.curWeaponGear.gearName;
          break;
        case "Head":
          if(bis)
            selectElement.value = this.player.bisHeadGear.gearName;
          else
            selectElement.value = this.player.curHeadGear.gearName;
          break;
        case "Hands":
          if(bis)
            selectElement.value = this.player.bisHandsGear.gearName;
          else
            selectElement.value = this.player.curHandsGear.gearName;
          break;
        case "Body":
          if(bis)
            selectElement.value = this.player.bisBodyGear.gearName;
          else
            selectElement.value = this.player.curBodyGear.gearName;
          break;
        case "Legs":
          if(bis)
            selectElement.value = this.player.bisLegsGear.gearName;
          else
            selectElement.value = this.player.curLegsGear.gearName;
          break;
        case "Feet":
          if(bis)
            selectElement.value = this.player.bisFeetGear.gearName;
          else
            selectElement.value = this.player.curFeetGear.gearName;
          break;
        case "Necklace":
          if(bis)
            selectElement.value = this.player.bisNecklaceGear.gearName;
          else
            selectElement.value = this.player.curNecklaceGear.gearName;
          break;
        case "Earrings":
          if(bis)
            selectElement.value = this.player.bisEarringsGear.gearName;
          else
            selectElement.value = this.player.curEarringsGear.gearName;
          break;
        case "Bracelets":
          if(bis)
            selectElement.value = this.player.bisBraceletsGear.gearName;
          else
            selectElement.value = this.player.curBraceletsGear.gearName;
          break;
        case "RightRing":
          if(bis)
            selectElement.value = this.player.bisRightRingGear.gearName;
          else
            selectElement.value = this.player.curRightRingGear.gearName;
          break;
        case "LeftRing":
          if(bis)
            selectElement.value = this.player.bisLeftRingGear.gearName;
          else
            selectElement.value = this.player.curLeftRingGear.gearName;
          break;
      }
      return false;
    }
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
    if (!bis && (NewGear.gearStage == "Upgraded_Tomes" || NewGear.gearStage == "Raid")){
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
          ////console.log("after closed")
          ////console.log(result)
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
      switch (GearType) {
        case "Weapon":
          oldGear = (bis ? this.player.bisWeaponGear : this.player.curWeaponGear);
          if (bis)
            this.player.bisWeaponGear = NewGear;
          else
            this.player.curWeaponGear = NewGear;
          break;
        case "Head":
          oldGear = (bis ? this.player.bisHeadGear : this.player.curHeadGear);
          if (bis)
            this.player.bisHeadGear = NewGear;
          else
            this.player.curHeadGear = NewGear;
          break;
        case "Hands":
          oldGear = (bis ? this.player.bisHandsGear : this.player.curHandsGear);
          if (bis)
            this.player.bisHandsGear = NewGear;
          else
            this.player.curHandsGear = NewGear;
          break;
        case "Body":
          oldGear = (bis ? this.player.bisBodyGear : this.player.curBodyGear);
          if (bis)
            this.player.bisBodyGear = NewGear;
          else
            this.player.curBodyGear = NewGear;
          break;
        case "Legs":
          oldGear = (bis ? this.player.bisLegsGear : this.player.curLegsGear);
          if (bis)
            this.player.bisLegsGear = NewGear;
          else
            this.player.curLegsGear = NewGear;
          break;
        case "Feet":
          oldGear = (bis ? this.player.bisFeetGear : this.player.curFeetGear);
          if (bis)
            this.player.bisFeetGear = NewGear;
          else
            this.player.curFeetGear = NewGear;
          break;
        case "Necklace":
          oldGear = (bis ? this.player.bisNecklaceGear : this.player.curNecklaceGear);
          if (bis)
            this.player.bisNecklaceGear = NewGear;
          else
            this.player.curNecklaceGear = NewGear;
          break;
        case "Earrings":
          oldGear = (bis ? this.player.bisEarringsGear : this.player.curEarringsGear);
          if (bis)
            this.player.bisEarringsGear = NewGear;
          else
            this.player.curEarringsGear = NewGear;
          break;
        case "Bracelets":
          oldGear = (bis ? this.player.bisBraceletsGear : this.player.curBraceletsGear);
          if (bis)
            this.player.bisBraceletsGear = NewGear;
          else
            this.player.curBraceletsGear = NewGear;
          break;
        case "RightRing":
          oldGear = (bis ? this.player.bisRightRingGear : this.player.curRightRingGear);
          if (bis)
            this.player.bisRightRingGear = NewGear;
          else
            this.player.curRightRingGear = NewGear;
          break;
        case "LeftRing":
          oldGear = (bis ? this.player.bisLeftRingGear : this.player.curLeftRingGear);
          if (bis)
            this.player.bisLeftRingGear = NewGear;
          else
            this.player.curLeftRingGear = NewGear;
          break;
      }
      var CheckPlayerLock = this.player.staticRef.LockParam["BOOL_LOCK_PLAYERS"];
      if (CheckPlayerLock && !this.player.staticRef.LockParam["LOCK_IF_TOME_AUGMENT"] && NewGear.gearStage === "Upgraded_Tomes")
        CheckPlayerLock=false;


      await this.http.changePlayerGear(this.player.id, GearTypeNumber, NewGear.id, bis, Turn, CheckPlayerLock, this.player.staticRef.useBookForGearAcq).pipe(catchError((error, s) => {
        ////console.log(error);
        this.unauthorized();
        // Reverting change
        selectElement.value = oldGear.gearName;
        switch(GearType){
          case "Weapon":
          if (bis)
            this.player.bisWeaponGear = oldGear;
          else
            this.player.curWeaponGear = oldGear;
          break;
        case "Head":
          if (bis)
            this.player.bisHeadGear = oldGear;
          else
            this.player.curHeadGear = oldGear;
          break;
        case "Hands":
          if (bis)
            this.player.bisHandsGear = oldGear;
          else
            this.player.curHandsGear = oldGear;
          break;
        case "Body":
          if (bis)
            this.player.bisBodyGear = oldGear;
          else
            this.player.curBodyGear = oldGear;
          break;
        case "Legs":
          if (bis)
            this.player.bisLegsGear = oldGear;
          else
            this.player.curLegsGear = oldGear;
          break;
        case "Feet":
          if (bis)
            this.player.bisFeetGear = oldGear;
          else
            this.player.curFeetGear = oldGear;
          break;
        case "Necklace":
          if (bis)
            this.player.bisNecklaceGear = oldGear;
          else
            this.player.curNecklaceGear = oldGear;
          break;
        case "Earrings":
          if (bis)
            this.player.bisEarringsGear = oldGear;
          else
            this.player.curEarringsGear = oldGear;
          break;
        case "Bracelets":
          if (bis)
            this.player.bisBraceletsGear = oldGear;
          else
            this.player.curBraceletsGear = oldGear;
          break;
        case "RightRing":
          if (bis)
            this.player.bisRightRingGear = oldGear;
          else
            this.player.curRightRingGear = oldGear;
          break;
        case "LeftRing":
          if (bis)
            this.player.bisLeftRingGear = oldGear;
          else
            this.player.curLeftRingGear = oldGear;
          break;
        }
        this.cdr.detectChanges();
        return of(null);
      }))
      .subscribe((data) => {
        ////console.log(data);
        this.RegetPlayerInfoSoft();
      });
      return true;
    }
    else{
      // Reverting change
      selectElement.value = oldGear.gearName;
      switch(GearType){
        case "Weapon":
        if (bis)
          this.player.bisWeaponGear = oldGear;
        else
          this.player.curWeaponGear = oldGear;
        break;
      case "Head":
        if (bis)
          this.player.bisHeadGear = oldGear;
        else
          this.player.curHeadGear = oldGear;
        break;
      case "Hands":
        if (bis)
          this.player.bisHandsGear = oldGear;
        else
          this.player.curHandsGear = oldGear;
        break;
      case "Body":
        if (bis)
          this.player.bisBodyGear = oldGear;
        else
          this.player.curBodyGear = oldGear;
        break;
      case "Legs":
        if (bis)
          this.player.bisLegsGear = oldGear;
        else
          this.player.curLegsGear = oldGear;
        break;
      case "Feet":
        if (bis)
          this.player.bisFeetGear = oldGear;
        else
          this.player.curFeetGear = oldGear;
        break;
      case "Necklace":
        if (bis)
          this.player.bisNecklaceGear = oldGear;
        else
          this.player.curNecklaceGear = oldGear;
        break;
      case "Earrings":
        if (bis)
          this.player.bisEarringsGear = oldGear;
        else
          this.player.curEarringsGear = oldGear;
        break;
      case "Bracelets":
        if (bis)
          this.player.bisBraceletsGear = oldGear;
        else
          this.player.curBraceletsGear = oldGear;
        break;
      case "RightRing":
        if (bis)
          this.player.bisRightRingGear = oldGear;
        else
          this.player.curRightRingGear = oldGear;
        break;
      case "LeftRing":
        if (bis)
          this.player.bisLeftRingGear = oldGear;
        else
          this.player.curLeftRingGear = oldGear;
        break;
      }
      this.cdr.detectChanges();
      return false;
    }
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
          ////console.log("after closed");
          ////console.log(result);
          resolve(result === "Yes"); // Resolve the promise with true if the result is "Yes"
        });
      } else {
        resolve(true); // If not locked, resolve the promise with true immediately
      }
    });
  }

  getImageSource(gear): string {
    if (gear.gearName == "No Equipment" || (gear === null))
      return 'assets/no_gear.webp';
    switch (gear.gearStage) {
      case 'Tomes':
        return 'assets/tomestone_icon.png';
      case 'Raid':
        return 'assets/raid_icon.webp';
      case 'Upgraded_Tomes':
          return 'assets/tomestone_icon_upgraded.png';
      case 'Extreme':
        return 'assets/extreme.webp';
      default:
        return 'assets/crafted_gear_icon.webp';
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
      data: {playerId, newEtro, IsEtro : false}
    }).afterClosed().subscribe(result => {
      //console.log("after closed")
      //console.log(result)
      if (result === "Yes"){this.RegetPlayerInfo();}
    });
  }

  onChangeName(event : Event){
    const selectElement = event.target as HTMLSelectElement;
    const value = selectElement.value;
    this.http.changePlayerName(this.player.id, value).pipe(catchError(error => {
      selectElement.value = this.player.name;
      this.unauthorized();
      return throwError(() => new Error('Failed to change player name: ' + error.message))
    }))
    .subscribe((data) => {
      this.player.name = value;
    });
  }
  onChangeJob(event : Event){
    const selectElement = event.target as HTMLSelectElement;
    const selectedIndex = selectElement.selectedIndex+1;
    this.http.changePlayerJob(this.player.id, selectedIndex).pipe(catchError(error => {
      this.player.job = this.oldJob;
      this.unauthorized();
      return throwError(() => new Error('Failed to change player job: ' + error.message))
    }))
    .subscribe((data) => {
      // Reset Job dependant values for the player.
      this.oldJob = this.player.job;
      this.http.resetPlayerJobDependantValues(this.player.id).subscribe((data) => {
        //console.log(data);
        this.RegetPlayerInfo();
    });
    });



  }



  RecomputePGSWholeStatic(){
      // Recomputing PGS
      this.http.RecomputePGS(this.player.staticRef.uuid).subscribe((data) => {
        //console.log(data);
        var data = data["playerGearScoreList"];
        for(var i = 0; i < data.length; i++){
          var index = this.player.staticRef.players.findIndex((player, b, c) => player.id === data[i].id);
          this.player.staticRef.players[index].playerGearScore = data[i].score;
        }
        
        this.staticEventsService.triggerRecomputePGS(); // Triggers event to recompute recommended order.
        this.cdr.detectChanges();
      });
  }

  RegetPlayerInfo(){
    this.http.GetSingletonPlayerInfo(this.player.staticRef.uuid, this.player.id).subscribe((data) => {
      //console.log(data);
      var newPlayer = Player.CreatePlayerFromDict(data);
      newPlayer.staticRef = this.player.staticRef;
      newPlayer.staticId = this.player.staticId;
      var index = this.player.staticRef.players.findIndex(player => player.id == this.player.id);
      this.player.job = newPlayer.job;
      this.player.staticRef.players[index] = newPlayer;
      this.player.GetGroupColorNoAlpha();
      this.RecomputePGSWholeStatic();
      this.cdr.detectChanges();
      
    });
  }

  RegetPlayerInfoSoft(){
    this.http.GetSingletonPlayerInfoSoft(this.player.id).subscribe((data) => {
      //console.log(data);
      this.player.LockedList = data["lockedList"].map(dateStr => new Date(dateStr));

      this.player.BisAverageItemLevel = data["averageItemLevelBis"];
      this.player.CurrentAverageItemLevel = data["averageItemLevelCurrent"];
      this.player.ShineCost = data["cost"]["shineCost"];
      this.player.TwineCost = data["cost"]["twineCost"];
      this.player.TomestoneCost = data["cost"]["tomeCost"];
      this.player.IsClaimed = data["isClaimed"];
      var index = this.player.staticRef.players.findIndex(player => player.id == this.player.id);
      this.player.staticRef.players[index] = this.player;
      this.RecomputePGSWholeStatic();
      this.player.GetGroupColorNoAlpha();
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
      //console.log("after closed")
      //console.log(result)
      if (result === "Yes"){
        this.http.RemoveLock(this.player.id, turn).pipe(catchError(error => {
          this.unauthorized();
          return throwError(() => new Error('Failed to remove lock : ' + error.message))
        }))
        .subscribe((data) => {
          ////console.log(data);
          this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
            duration: 3500,
            data: {
              message: "Successfuly removed lock",
              subMessage: "",
              color : "green"
            }
          });
          this.RegetPlayerInfoSoft();
        });
      }
    });
  }

  onMouseEnter(turn : number, event: any) {
    var check = this.player.IsLockedOutOfTurn(turn);
    ////console.log(check);
    if (check){
      event.target.style.cursor = 'pointer';
      event.target.style.filter = 'brightness(1.1)';
    }
  }
  onMouseLeave(event: any) {
    event.target.style.filter = '';
  }

  onEtroBiSChange(){
    // Check to only put uuid
    if (this.player.etroBiS.includes("etro.gg/gearset/")) {
      this.player.etroBiS = this.player.etroBiS.split("etro.gg/gearset/")[1];
  }
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
  MatDialogModule,
} from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { DomSanitizer } from '@angular/platform-browser';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PizzaPartyAnnotatedComponent } from '../static-detail/static-detail.component';
import { catchError, of, throwError } from 'rxjs';
import { StaticEventsService } from '../service/static-events.service';
import { etroToolTip, gearSelectionToolTip, lockOnPlayerToolTip, pgsOnPlayerToolTip } from '../tooltip';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'import-etro',
  templateUrl: './import-etro.component.html',
  standalone: true,
  imports: [MatButtonModule, MatDialogActions, MatDialogClose, MatDialogTitle, MatDialogContent, CommonModule, MatDialogModule],
})
export class EtroDialog {
  constructor(public dialogRef: MatDialogRef<EtroDialog>,public http: HttpService,
    @Inject(MAT_DIALOG_DATA) public data: { playerId: number; newEtro: string, oldEtro : string, IsEtro : boolean },
    private sanitizer: DomSanitizer, private _snackBar: MatSnackBar, private httpClient : HttpClient
  ) {}
  public isLoading : boolean = false;
  public xivGearUncertainty : boolean = false;
  public xivGearUniqueName : string = "";
  public selectedxivGearSet : number = 0;
  public xivGearSelection : any = [];
  getSafeUrl(){
  let unsafeUrl = 'https://etro.gg/embed/gearset/' + this.data.newEtro;
  return this.sanitizer.bypassSecurityTrustResourceUrl(unsafeUrl);
  }

  ngOnInit(){
    if (!this.data.IsEtro){
      // Must check if more than one set.
      this.isLoading = true;
      var uuid = this.data.newEtro.split("xivgear.app/?page=sl%7C")[1];
      this.httpClient.get("https://api.xivgear.app/shortlink/" + uuid).subscribe((data) => {
        if ('items' in data){
          this.xivGearUniqueName = data['name'];
          this.isLoading = false;return;
        } else if ('sets' in data){
          this.xivGearUncertainty = true;
          var setList : any = data['sets'];
          for (let set in setList){
            this.xivGearSelection.push([setList[set]['name'], set]);
          }
          this.isLoading = false;return;
        }
      });
    }
  }

  onSelectXIVGearSet(id : number){
    this.xivGearSelection = id;
  }

  unauthorized(){
    this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
      duration: 3500,
      data: {
        message: "You have not claimed this player. Only its owner can modify it.",
        subMessage: "The changes will be reverted",
        color : "red"
      }
    });
  }

  ImportEtro(playerId : number, newEtro : string){
    this.isLoading = true;
    this.http.changePlayerEtro(playerId, newEtro).pipe(
      catchError(err => {
        if (err.status === 401) {
          this.unauthorized();
        } else{
          this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
            duration: 8000,
            data: {
              message: "Error while trying to import from etro.",
              subMessage: "(Make sure the UUID is correct)",
              color : "red"
            }
          });
        }
        return throwError(() => new Error('Failed to import etro : ' + err.message))
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

  ImportXIVGear(playerId : number, link : string){

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
