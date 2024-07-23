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

  public GearBreakdownToolTip = GearBreakdownToolTip;

  public curSelectedTurn : string = "turn_1";
  public curSelectTurnName : string = "AAC Light-heavyweight M1S"
  public gearOrderByTurn : any = {
    "turn_1" : ["Earrings", "Necklace", "Bracelets", "Ring"],
    "turn_2" : ["Head", "Hands", "Feet", "Shine"],
    "turn_3" : ["Head","Body", "Hands", "Legs", "Feet","Twine"],
    "turn_4" : ["Weapon"]
  };

  updateSelectedTurn(turn:string){
    this.curSelectedTurn = turn;
    switch(this.curSelectedTurn){
      case "turn_1":
        this.curSelectTurnName = "AAC Light-heavyweight M1S";
        break;
        case "turn_2":
        this.curSelectTurnName = "AAC Light-heavyweight M2S";
        break;
      case "turn_3":
        this.curSelectTurnName = "AAC Light-heavyweight M3S";
        break;
      case "turn_4":
        this.curSelectTurnName = "AAC Light-heavyweight M4S";
        break;
    }
  }

  getBackgroundColor(id : number, turn : string){
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

  /*
    {turn1 : "Earrings" : [("Leonhard Euler", true). ("Harrow Levesque", true)]}
  */
}
