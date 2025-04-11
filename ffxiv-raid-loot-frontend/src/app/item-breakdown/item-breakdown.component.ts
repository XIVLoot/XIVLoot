import { Component, Input } from '@angular/core';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { GearBreakdownToolTip } from '../tooltip';
@Component({
  selector: 'app-item-breakdown',
  templateUrl: './item-breakdown.component.html',
  styleUrl: './item-breakdown.component.css'
})
export class ItemBreakdownComponent {

  @Input({required:true}) itemBreakdownInfo! : any;
  @Input({required:true}) playerList! : any;
  @Input({required:true}) tier! : number;

  public GearBreakdownToolTip = GearBreakdownToolTip;

  public curSelectedTurn : string = "turn_1";
  public curSelectTurnName : string = "AAC Light-heavyweight M1S";
  public gearOrderByTurn : any = {
    "turn_1" : ["Earrings", "Necklace", "Bracelets", "Ring"],
    "turn_2" : ["Head", "Hands", "Feet", "Shine"],
    "turn_3" : ["Body", "Legs","Twine"],
    "turn_4" : ["Weapon"]
  };

  updateSelectedTurn(turn:string){
    this.curSelectedTurn = turn;
    this.curSelectTurnName = this.getTurnName();
  }

  getTurnName() : string
  {
    switch(this.tier){
      case 0:
        switch(this.curSelectedTurn){
          case "turn_1":
            return "AAC Light-heavyweight M1S";
            case "turn_2":
              return "AAC Light-heavyweight M2S";
          case "turn_3":
            return "AAC Light-heavyweight M3S";
          case "turn_4":
            return "AAC Light-heavyweight M4S";
        }
        break;
      case 1:
          switch(this.curSelectedTurn){
            case "turn_1":
              return "AAC Cruiserweight M5S";
              case "turn_2":
                return "AAC Cruiserweight M6S";
            case "turn_3":
              return "AAC Cruiserweight M7S";
            case "turn_4":
              return "AAC Cruiserweight M8S";
          }
          break;
      case 2:
          switch(this.curSelectedTurn){
            case "turn_1":
              return"No Name";
            case "turn_2":
              return "No Name";
            case "turn_3":
              return "No Name";
            case "turn_4":
              return "No Name";
            }
            break;
    }
    return this.curSelectTurnName;
  }
  
  getBackgroundColor(id : number, turn : string){
    var player = this.playerList.find(player => player.id === id);
    if (player.IsAlt){
      return 'rgba(255, 140, 0, 0.6)';
    }
    var turnInt : number = 0;
    switch(turn){
      case "turn_1":
        turnInt = 1;
        break;
      case "turn_2":
        turnInt = 2;
        break;
      case "turn_3":
        turnInt = 3;
        break;
      case "turn_4":
        turnInt = 4;
        break;
    }
    return this.playerList.find(player => player.id === id).IsLockedOutOfTurn(turnInt) ? 'rgba(255,0,0,0.4)' : 'rgba(0,255,0,0.2)'
  }

  getLockIcon(id : number, turn : string){
    var turnInt : number = 0;
    switch(turn){
      case "turn_1":
        turnInt = 1;
        break;
      case "turn_2":
        turnInt = 2;
        break;
      case "turn_3":
        turnInt = 3;
        break;
      case "turn_4":
        turnInt = 4;
        break;
    }
    return this.playerList.find(player => player.id === id).IsLockedOutOfTurn(turnInt) ? 'lock_outline' : 'lock_open'
  }
  
  getJobIcon(id : number){
    return this.playerList.find(player => player.id === id).job;
  }

  getCofferIcon(gear : string){
    switch(gear){
      case "Ring":
        return "assets/gear_icon/LeftRing_chest.png";
      case "Twine":
        return "assets/twine_icon.png";
      case "Shine":
        return "assets/shine_icon.png";
      default:
        return "assets/gear_icon/"+gear+"_chest.png";
    }
  }


  getTierName()
  {
    switch(this.tier){
      case 0:
        return "Arcadion tier 1";
      case 1:
        return "Arcadion tier 2";
      case 2:
        return "Arcadion tier 3";
      default:
        return "Unknown tier";
    }
  }

  getTurnImage(turn : number){
    switch(turn){
      case 1:
        switch (this.tier){
          case 2:
            return "assets/raid/no_image.png";
          case 1:
            return "assets/raid/no_image.png";
          case 0:
            return "assets/raid/turn_1_d.png";
        }
      case 2:
        switch (this.tier){
          case 2:
            return "assets/raid/no_image.png";
          case 1:
            return "assets/raid/no_image.png";
          case 0:
            return "assets/raid/turn_2_d.png";
        }
      case 3:
        switch (this.tier){
          case 2:
            return "assets/raid/no_image.png";
          case 1:
            return "assets/raid/no_image.png";
          case 0:
            return "assets/raid/turn_3_d.png";
        }
      case 4:
        switch (this.tier){
          case 2:
            return "assets/raid/no_image.png";
          case 1:
            return "assets/raid/no_image.png";
          case 0:
            return "assets/raid/turn_4_d.png";
        }
  }
}
  /*
    {turn1 : "Earrings" : [("Leonhard Euler", true). ("Harrow Levesque", true)]}
  */
}
