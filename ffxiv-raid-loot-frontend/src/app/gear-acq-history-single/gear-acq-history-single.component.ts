import { Component, Input } from '@angular/core';
import { Static } from '../models/static';


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
          isAugment : v.isAugment
        });
      } else {
        this.ListOfTurnInfo[v.turn][playerName] = [{
          gearType : v.gearType,
          isAugment : v.isAugment
        }];
      }
    }
    for (let key of Object.keys(this.ListOfTurnInfo)){
      var x = Object.keys(this.ListOfTurnInfo[parseInt(key)]).length;
      this.ListOfTurnInfoNumber.push(x);
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
          return "assets/twine_icon.png";
        default:
          return "assets/shine_icon.png";
      }
    }
    return "assets/gear_icon/"+v.gearType+"_chest.png";
  }

}
