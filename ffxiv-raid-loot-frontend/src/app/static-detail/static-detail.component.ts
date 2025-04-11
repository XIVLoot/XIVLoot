import { Component, OnInit, HostListener, Inject, ChangeDetectorRef } from '@angular/core';
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
import {MatSliderModule} from '@angular/material/slider';
import {MatCardModule} from '@angular/material/card';

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
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { environment } from '../../environments/environments';
import { Player } from '../models/player';
import { gearAcquisitionToolTip, pgsSettingToolTipA, pgsSettingToolTipB, pgsSettingToolTipC, pgsToolTip, lockLogicToolTip, lockOutOfGearEvenIfNotContestedToolTip,
  lockPerFightToolTip, lockPlayerForAugmentToolTip, pieceUntilLockToolTip, numberWeekResetToolTip,addNewPlayerToolTip,swapAltPlayerToolTip,deletePlayerToolTip,
  claimPlayerToolTip,
  unclaimPlayerToolTip,
  alreadyClaimedToolTip, UseBookForGearAcqToolTip, FreePlayerToolTip,
  ClaimStaticToolTip,
  UnclaimStaticToolTip
} from '../tooltip';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ConfirmDialog } from '../player-details-single/player-details-single.component';

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

  public gearAcquisitionToolTip = gearAcquisitionToolTip;
  public pgsToolTip = pgsToolTip;
  public lockLogicToolTip = lockLogicToolTip;
  public lockOutOfGearEvenIfNotContestedToolTip = lockOutOfGearEvenIfNotContestedToolTip;
  public lockPerFightToolTip = lockPerFightToolTip;
  public lockPlayerForAugmentToolTip = lockPlayerForAugmentToolTip;
  public pieceUntilLockToolTip = pieceUntilLockToolTip;
  public numberWeekResetToolTip = numberWeekResetToolTip;
  public claimPlayerToolTip = claimPlayerToolTip;
  public unclaimPlayerToolTip = unclaimPlayerToolTip;
  public alreadyClaimedToolTip = alreadyClaimedToolTip;
  public useBookForGearAcqToolTip = UseBookForGearAcqToolTip;
  public FreePlayerToolTip = FreePlayerToolTip;
  public ClaimStaticToolTip = ClaimStaticToolTip;
  public UnclaimStaticToolTip = UnclaimStaticToolTip;
  public addNewPlayerToolTip = addNewPlayerToolTip;
  public swapAltPlayerToolTip= swapAltPlayerToolTip;
  public deletePlayerToolTip = deletePlayerToolTip;

  public staticDetail: Static; // Holds the details of a static
  public uuid: string; // UUID of the static
  public gridColumns = 3; // Default number of grid columns
  public groupList = [];
  public test : boolean = true;
  public OriginalLockParam : any;
  public LockParamChangeCheck : boolean = false;
  public ShowAllPlayer : boolean = false;
  public SelectedPlayer : number;
  public ShowNumberLastWeekHistory : number = 4;
  public ShowAllHistory : boolean = false;
  public GearAcqHistory : Object = {};
  public HistoryGear : any = [];
  public IsLoading : boolean = true;
  public userOwns : any = {};
  public staticLeaderName : string = "";
  public PlayerListPerShower : any = [[],[],[],[],[],[],[],[]]; // Each sublist contains all players for a shower
  public selectedPlayerSubList : number = 1;
  public selectedPlayerSubListMax : number = 1;

  public UserIsOwnerOfStatic : boolean = false;

  public itemBreakdownInfo : any = {
    "turn_1" : {},
    "turn_2" : {},
    "turn_3" : {},
    "turn_4" : {}
  };

  public curViewingTool : string = "GearBrk";

  IncrementselectedPlayerSubList(){
    if (this.selectedPlayerSubList < this.selectedPlayerSubListMax-1){
      this.selectedPlayerSubList++;
    }
  }

  getTurnImage(turn : number){
    switch(turn){
      case 1:
        switch (this.staticDetail.Tier){
          case 2:
            return "assets/raid/no_image.png";
          case 1:
            return "assets/raid/no_image.png";
          case 0:
            return "assets/raid/turn_1_d.png";
        }
      case 2:
        switch (this.staticDetail.Tier){
          case 2:
            return "assets/raid/no_image.png";
          case 1:
            return "assets/raid/no_image.png";
          case 0:
            return "assets/raid/turn_2_d.png";
        }
      case 3:
        switch (this.staticDetail.Tier){
          case 2:
            return "assets/raid/no_image.png";
          case 1:
            return "assets/raid/no_image.png";
          case 0:
            return "assets/raid/turn_3_d.png";
        }
      case 4:
        switch (this.staticDetail.Tier){
          case 2:
            return "assets/raid/no_image.png";
          case 1:
            return "assets/raid/no_image.png";
          case 0:
            return "assets/raid/turn_4_d.png";
        }
    }
  }

  DecrementselectedPlayerSubList(){
    if (this.selectedPlayerSubList > 0){
      this.selectedPlayerSubList--;
    }
  }

  GeneratePlayerListShower(){
            //Generate player list shower
    this.PlayerListPerShower = [[],[],[],[],[],[],[],[]];
    var playerCounter = 0;
    for (let player of this.staticDetail.players){
      this.PlayerListPerShower[playerCounter%8].push([Math.floor(playerCounter/8), player]);
      playerCounter++;
    }
    this.selectedPlayerSubListMax = Math.ceil(playerCounter/8);


    // Now fill the rest of the sublist with empty players
    for (let i = playerCounter; i < 8*this.selectedPlayerSubListMax; i++){
      this.PlayerListPerShower[i%8].push([this.selectedPlayerSubListMax-1, "None"]);
    }
  }

  AddNewPlayerToStatic(){
    this.http.AddNewPlayerToStatic(this.uuid).subscribe(data => {
      this.staticDetail.players.push(Player.CreatePlayerFromDict(data));
      this.GeneratePlayerListShower();
    });
  }

  changeCurViewingTool(newtool : string){
    this.curViewingTool=newtool;
  }

  constructor(public http: HttpService, private route: ActivatedRoute, private _snackBar: MatSnackBar,
    private staticEventsService: StaticEventsService, private dialog : MatDialog, private cdr: ChangeDetectorRef
  ) {
    this.staticEventsService.recomputePGS$.subscribe(() => {
      this.groupList = this.ComputeNumberPGSGroup();
    });
   } // Constructor with dependency injection

   async UnclaimPlayer(player : Player){
    var id = player.id;
    var DiscordLoggedIn = await this.http.CheckAuthDiscord();
    var DefaultLoggedIn;
    try {
      DefaultLoggedIn = await this.http.CheckAuthDefault();
    } catch (error) {
      DefaultLoggedIn = false;
    }
    if (!DefaultLoggedIn && !DiscordLoggedIn){
      return false;
    }
    
    if(DiscordLoggedIn){
      this.http.GetDiscordUserInfo().subscribe(data => {
        this.http.UnclaimPlayerDiscord(data["id"], id).subscribe(res => {
          player.IsClaimed = false;
          player.staticRef.userOwn[id] = false;
          this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
            duration: 3500,
            data : {
              message : "Successfully unclaimed the player", 
              subMessage : "",
              color : "Green"
            }
          });
        });
      });
    } else if (DefaultLoggedIn){
      this.http.UnclaimPlayerDefault(id).subscribe(res => {
        player.IsClaimed = false;
        player.staticRef.userOwn[id] = false;
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 3500,
          data : {
            message : "Successfully unclaimed the player", 
            subMessage : "",
            color : "Green"
          }
        });
      });
    }

   }

   FreePlayer(player : Player){
    var id = player.id;
    this.http.FreePlayer(player.staticRef.uuid, player).subscribe(data => {
      player.IsClaimed = false;
      player.staticRef.userOwn[id] = false;
      this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
        duration: 3500,
        data : {
          message : "Successfully freed the player", 
          subMessage : "",
          color : "Green"
        }
      });
    });
   }

   async ClaimPlayer(player : Player){
    var id = player.id;
    var DiscordLoggedIn = await this.http.CheckAuthDiscord();
    var DefaultLoggedIn;
    try {
      DefaultLoggedIn = await this.http.CheckAuthDefault();
    } catch (error) {
      DefaultLoggedIn = false;
    }
    if (!DefaultLoggedIn && !DiscordLoggedIn){
      this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
        duration: 3500,
        data: {
          message: "Login to claim the player",
          subMessage: " ",
          color : "yellow"
        }
      });
      return false;
    }
    
    if(DiscordLoggedIn){
      this.http.GetDiscordUserInfo().subscribe(data => {
        this.http.ClaimPlayerDiscord(data["id"], id).subscribe(res => {
          player.IsClaimed = true;
          player.staticRef.userOwn[id] = true;
          this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
            duration: 3500,
            data : {
              message : "Successfully claimed the player", 
              subMessage : "",
              color : "Green"
            }
          });
        });
      });
    } else if (DefaultLoggedIn){
      this.http.ClaimPlayerDefault(id).subscribe(res => {
        player.IsClaimed = true;
        player.staticRef.userOwn[id] = true;
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 3500,
          data : {
            message : "Successfully claimed the player", 
            subMessage : "",
            color : "Green"
          }
        });
      });
    }
   }

   SwapAltPlayer(player : Player){
    this.http.SwapAltPlayer(player).subscribe(res => {
      //player.IsAlt = res === "true";
    });
   }

   async CheckClaimPlayer(id : number){
    var DiscordLoggedIn = await this.http.CheckAuthDiscord();
    var DefaultLoggedIn;
    try {
      DefaultLoggedIn = await this.http.CheckAuthDefault();
    } catch (error) {
      DefaultLoggedIn = false;
    }
    if (!DefaultLoggedIn && !DiscordLoggedIn){
      return new Promise<boolean>(resolve => resolve(false));
    }
    
    if(DiscordLoggedIn){
      return new Promise<boolean>(resolve => {
        this.http.GetDiscordUserInfo().subscribe(data => {
          this.http.IsPlayerClaimedByUserDiscord(data["id"], id.toString()).subscribe(res => resolve(!!res)) // Boolean casting??
        });
      });

    } else if (DefaultLoggedIn){
      return new Promise<boolean>(resolve => {
        this.http.IsPlayerClaimedByUserDefault(id.toString()).subscribe(res => resolve(!!res));
      });
    }
    return new Promise<boolean>(resolve => resolve(false));
   }

     // Method to check if URL contains a UUID
  containsUUID(url: string): boolean {
    const uuidRegex = /[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}/;
    return uuidRegex.test(url);
  }

  async ngOnInit() {
    this.test = true;
    this.staticDetail = new Static(0, "", "",0, [], {});

    // Subscribe to route parameters to get the 'uuid'
    this.route.params.subscribe(params => {
      this.uuid = params['uuid'];
    });

    if (this.containsUUID(window.location.href.split('?')[0])){
      var curUUID = window.location.href.split(environment.site_url)[1].split('?')[0];
      
      if (localStorage.getItem('recentStatic') !== null){
        var retrievedList = JSON.parse(localStorage.getItem('recentStatic') || '[]');
        if (!retrievedList.includes(curUUID))
          {
          retrievedList.unshift(curUUID);

          if (retrievedList.length >= 10){
            retrievedList.pop();
          }

          localStorage.setItem('recentStatic', JSON.stringify(retrievedList));

          }
      } else{
        localStorage.setItem('recentStatic', JSON.stringify([curUUID]));
      }
    }


    this.dialog.open(LoadingDialogComponent, {
      //disableClose:true,
      data : {uuid : this.uuid}
    });

    //console.log("Trying details");
    // Fetch static details from the server using the uuid
    try {
      this.http.getStatic(this.uuid).subscribe(details => {
        ////console.log("Received details");
        this.staticDetail = details; // Assign the fetched details to staticDetail
        this.OriginalLockParam = JSON.parse(JSON.stringify(this.staticDetail.LockParam)); // Deepcopy
        ////console.log(this.staticDetail); // Log the static details to the console
        this.groupList = this.ComputeNumberPGSGroup();

        this.GeneratePlayerListShower();
        this.selectedPlayerSubList = 0;

        this.http.GetGearAcqHistory(this.uuid, this.ShowNumberLastWeekHistory).subscribe(async data => {
          this.GearAcqHistory = data["info"];
          const keys = Object.keys(this.GearAcqHistory);

          for (let x = keys.length-1;x>=0;x--){
            this.HistoryGear.push(keys[x]);
          }
          this.staticDetail.userOwn = {};
          for(let player of this.staticDetail.players){
            this.CheckClaimPlayer(player.id).then((result: boolean) => {
              this.staticDetail.userOwn[player.id] = result;
          });;
          }

          const urlParams = new URLSearchParams(window.location.search);
          const pId = urlParams.get('pId');
          if (pId) {
              this.SelectedPlayer = parseInt(pId);
          }

          this.http.GetItemBreakdownInfo(this.uuid).subscribe(rData => {
            this.itemBreakdownInfo = rData.itemBreakdown;
            this.http.UserOwnStatic(this.uuid).subscribe(pData => {
              this.UserIsOwnerOfStatic = (pData.toLowerCase() === 'true');
              console.log("This static is owned : " + this.UserIsOwnerOfStatic);
              this.http.GetOwnerName(this.uuid).subscribe(datar => {
                this.staticLeaderName = datar
                this.IsLoading = false;
                this.dialog.closeAll();
                this.cdr.detectChanges();
              });

            });
          });



      });
      });
    } catch (error){
      this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
        duration: 3500,
        data: {
          message: "An error occured while loading the static.",
          subMessage: "if you see any issues please reach out.",
          color: "red"
        }
      });
      this.IsLoading = false;
      this.dialog.closeAll();
      this.cdr.detectChanges();
    }
    this.onResize(null); // Call onResize to set initial gridColumns based on window size
  }

  UnclaimThisStatic(){
    this.http.UnclaimStaticOwnerShip(this.uuid).subscribe(data => {
      if (data === "true"){
        this.staticLeaderName = "REFRESH TO SEE";
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 3500,
          data: {
            message: "Successfuly Unclaimed static.",
            subMessage: "",
            color : "green"
          }
        });
        this.UserIsOwnerOfStatic = false;
        this.staticLeaderName = "";
      } else {
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 8000,
          data: {
            message: "Failed to unclaim static.",
            subMessage: "Reach out.",
            color : "red"
          }
        });
      }
    });
  }

  ClaimThisStatic(){
    this.http.ClaimStaticOwnerShip(this.uuid).subscribe(data =>{
      if (data === "true"){
        
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 3500,
          data: {
            message: "Successfuly Claimed static.",
            subMessage: "",
            color : "green"
          }
        });
        this.staticLeaderName = "REFRESH TO SEE";
        this.UserIsOwnerOfStatic = true;
      } else {
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 8000,
          data: {
            message: "Failed to claim static.",
            subMessage: "Make sure you have claimed a player from this static and are logged in.",
            color : "red"
          }
        });
      }
    });
  }

  ChangeHistoryLoaded(){
    if (this.ShowNumberLastWeekHistory > 16){
      this.ShowNumberLastWeekHistory = 16;
    }
    if (this.ShowNumberLastWeekHistory < 1){
      this.ShowNumberLastWeekHistory = 1;
    }
    if (!Number.isInteger(this.ShowNumberLastWeekHistory)){
      this.ShowNumberLastWeekHistory = Math.round(this.ShowNumberLastWeekHistory);
    }
    this.http.GetGearAcqHistory(this.uuid, this.ShowNumberLastWeekHistory).subscribe(data => {
      // TODO : THIS UPADATE IS NOT VERY EFFICIENT
      this.GearAcqHistory = data["info"];
      this.HistoryGear = [];
      const keys = Object.keys(this.GearAcqHistory);

      for (let x = keys.length-1;x>=0;x--){
        this.HistoryGear.push(keys[x]);
      }

      this.cdr.detectChanges();
  });
  }

  CheckChange(){
    for (let key in this.OriginalLockParam){
      if (this.OriginalLockParam[key] !== this.staticDetail.LockParam[key]){
        this.LockParamChangeCheck=true;return;
      }
    }
    this.LockParamChangeCheck=false;
  }

  SaveLockParam(){
    this.http.updateStaticLockParam(this.staticDetail.uuid, this.staticDetail.LockParam).subscribe(res => {
      //console.log(res);
      this.OriginalLockParam = JSON.parse(JSON.stringify(this.staticDetail.LockParam)); // Deepcopy
      this.LockParamChangeCheck=false;
      this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
        duration: 3500,
        data: {
          message: "Successfuly updated parameters.",
          subMessage: "",
          color : "green"
        }
      });
    });
  }

  async SaveStaticToUser(){

    var DiscordLoggedIn = await this.http.CheckAuthDiscord();
    var DefaultLoggedIn;
    try {
      DefaultLoggedIn = await this.http.CheckAuthDefault();
    } catch (error) {
      DefaultLoggedIn = false;
    }
    if (!DefaultLoggedIn && !DiscordLoggedIn){
      this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
        duration: 3500,
        data: {
          message: "Login to save a static.",
          subMessage: " ",
          color : "yellow"
        }
      });
      return false;
    } 
    if (DiscordLoggedIn){
      this.http.GetDiscordUserInfo().subscribe(data => {
        this.http.SaveStaticToUserDiscord(data['id'], this.staticDetail.uuid).subscribe(res => {
          //console.log(res);
          this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
            duration: 3500,
            data: {
              message: "Successfuly saved static!",
              subMessage: "",
              color : "green"
            }
          });
        });
      });
    } else if (DefaultLoggedIn){
      this.http.SaveStaticToUserDefault(this.staticDetail.uuid).subscribe(res => {
        //console.log(res);
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 3500,
          data: {
            message: "Successfuly saved static!",
            subMessage: "",
            color : "green"
          }
        });
      });
    }

  }

  onMouseEnter(event: any) {
    event.target.style.outline = "2px solid rgba(255, 255, 255, 0.7)";
    event.target.style.cursor = 'pointer';
  }
  onMouseLeave(event: any) {
    event.target.style.outline = "";
  }

  selectPlayer(player : any){
    if (player !== "None"){
      this.SelectedPlayer = player.id;
    }
  }

  DeletePlayer(player : any){

    if(!player.IsAlt){
      // Can only delete alts
      return;
    }



    this.dialog.open(ConfirmDialog, {
      width: '500px',
      height: '200px',
      data: {title : "Delete Player", content : "Are you sure you want to permenantly delete this player ("+player.name +")?", yes_option : "Yes", no_option : "No", subContent : "THIS ACTION IS IRREVERSIBLE.",}
    }).afterClosed().subscribe(result => {
      if (result === "Yes"){
        this.http.DeletePlayer(player.id).subscribe(res => {
          for (let puck of this.PlayerListPerShower){
            for (let i = 0;i<puck[this.selectedPlayerSubList].length;i++){
              if (puck[this.selectedPlayerSubList][i].id === player.id){
                //Found player
                if (player.id === this.SelectedPlayer){
                  this.SelectedPlayer = 0;
                }
                puck[this.selectedPlayerSubList].splice(i, 1);
                puck[this.selectedPlayerSubList].push("None");
                
              }
            }
          }
        });
      }
    });
  }

  getJobIcon(job : string){
    return `assets/job/${job}.png`;
  }

  getBackgroundColor(job : string){
    switch(job){
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
      default:
        return "rgba(0, 0, 0, 0)";
    }
  }

  onChangeStaticName(event : Event){
    const selectElement = event.target as HTMLSelectElement;
    const newValue = selectElement.value;
    this.http.ChangeStaticName(this.staticDetail.uuid, newValue).subscribe(res => {
      //console.log(res);
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

    let PGSList = [];
    let groupList = [];
    for (let i = 0;i<this.staticDetail.players.length;i++){
      if (!this.staticDetail.players[i].IsAlt){
        PGSList.push(this.staticDetail.players[i]);
      }
    }
    let highestPGS = Math.max(...PGSList.map(player => player.playerGearScore));
    let lowestPGS = Math.min(...PGSList.map(player => player.playerGearScore));

    let tol = (highestPGS - lowestPGS)/4;

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
        PGSList[i].PGSGroupNumber = nGroup;
      }
      playerGroupList.push({group : curGroup, nGroup : nGroup});
      nGroup+=1;
    }
    for(let i = playerGroupList.length;i<4;i++){
      playerGroupList.push({group : [], nGroup : i}) // change to 2*i if want no color
    }
    return playerGroupList;
  }

  GetGroupOfPlayer(player : Player){
    let playerGroupList = this.ComputeNumberPGSGroup();
    for (let group of playerGroupList){
      if (group.group.includes(player)){
        return group.nGroup;
      }
    }
    return -1;
  }

  GetGroupColor(nGroup : number){
    switch(nGroup){
      case -1:
        return 'rgba(0, 0, 0, 0.3)';
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
    const valueToCopy = environment.site_url+this.staticDetail.uuid;
    navigator.clipboard.writeText(valueToCopy)
      .then(() => {
        // Handle successful copying here, e.g., show a message
        //console.log('UUID copied to clipboard!');
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 3500,
          data : {
            message : "Copied to clipboard!", 
            subMessage : "(Send the link to your friends for them to access the static)",
            color : "green"
          }
        });
      })
      .catch(err => {
        // Handle errors here
        console.error('Failed to copy UUID: ', err);
      });
  }


  openSettingPGS(){
    this.dialog.open(SettingPGS, {
      width: '500px',
      height: '430px',
      data: {uuid : this.staticDetail.uuid}
    }).afterClosed().subscribe(result => {
      //console.log("after closed")
      //console.log(result)
    });
  }

}

@Component({
  selector: 'SnackBar',
  template: `
   <div [style.background-color]="data.color" style="width: 120%; height: 100%; position: relative;box-shadow: 5px 4px 6px rgba(0, 0, 0, 0.7);border-radius:10px;">
      <h2>{{ data.message }}</h2>
      <p>{{ data.subMessage }}</p>
      <mat-icon (click)="snackBarRef.dismissWithAction()" style="position:absolute;top:5px;right:5px;cursor:pointer;">close</mat-icon>
    </div>

  `,
  imports: [MatIconModule],
  standalone:true,
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
  ::ng-deep .mdc-snackbar__label {
    padding: 0px;
    margin: 0px;
    
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
  imports: [MatButtonModule, MatDialogActions, MatDialogClose, MatDialogTitle, MatDialogContent, MatSliderModule,
    MatCardModule, FormsModule, MatTooltipModule, MatIcon
  ]
})
export class SettingPGS {
  constructor(public dialogRef: MatDialogRef<SettingPGS>,public http: HttpService,
    @Inject(MAT_DIALOG_DATA) public data: { uuid : string },
    private sanitizer: DomSanitizer, private _snackBar: MatSnackBar
  ) {}
  public pgsToolTipA : string = pgsSettingToolTipA;
  public pgsToolTipB : string = pgsSettingToolTipB;
  public pgsToolTipC : string = pgsSettingToolTipC;
  public disabled = false;
  public max = 100;
  public min = 0;
  public showTicks = false;
  public step = 1;
  public thumbLabel = false;
  public value_a : number = 0;
  public value_b : number = 0;
  public value_c : number = 0;


  ngOnInit(){
    this.http.GetPGSParam(this.data.uuid).subscribe(data => {
      this.value_a = Math.floor(data[0] * 10);
      this.value_b = Math.floor(data[1] * 10);
      this.value_c = Math.floor(data[2] * 10);
    });
  }

  UseRecommend(){
    this.value_a = 35;
    this.value_b = 25;
    this.value_c = 40;
  }

  SaveChange(){
    this.http.SetPGSParam(this.data.uuid, this.value_a/10, this.value_b/10, this.value_c/10).subscribe(data => {
      //console.log(data);
      this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
        duration: 3500,
        data : {
          message : "Successfully updated PGS settings", 
          subMessage : "(Reload the page to see the changes)",
          color : "green"
        }
      });
      this.dialogRef.close()});
  }

}

@Component({
  selector: 'app-loading-dialog',
  template: `
    <div style="text-align: center; padding: 20px;">
      <h1 style="color:white;">Loading Static : {{data.uuid}}</h1>
      <img src="assets/loading_gif.gif" style="width:100px;height:100px;">
      <p>(Click out to close if it is stuck on loading)</p>
    </div>
  `,
})
export class LoadingDialogComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: { uuid : string },){}
}

