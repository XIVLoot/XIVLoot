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

  public ListOfTurnInfo : any = {
    1 : {},
    2 : {},
    3 : {},
    4 : {}
  }

  ngOnInit(){
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
    var x = 0;
  }

}
