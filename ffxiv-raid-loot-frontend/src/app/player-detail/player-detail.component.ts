import { Component, Input } from '@angular/core';
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
  constructor(public http: HttpService, private route: ActivatedRoute) { } // Constructor with dependency injection
  onChangeGear(GearType : string, bis : boolean, event: Event){
    const selectElement = event.target as HTMLSelectElement;
    const selectedIndex = selectElement.selectedIndex;
    var NewGear : Gear;
    var GearTypeNumber : number;
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
        GearTypeNumber = 3;
        break;
      case "Body":
        NewGear = this.player.BodyChoice[selectedIndex];
        GearTypeNumber = 4;
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
        GearTypeNumber = 7;
        break;
      case "Earrings":
        NewGear = this.player.EarringsChoice[selectedIndex];
        GearTypeNumber = 8;
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
    this.http.changePlayerGear(this.player.id, GearTypeNumber, NewGear.id, bis).subscribe((data) => {
      console.log(data);
    });
    
  }
  onChangeEtro(event : Event){
    const selectElement = event.target as HTMLSelectElement;
    const newEtro = selectElement.value;
    this.http.changePlayerEtro(this.player.id, newEtro).subscribe((data) => {
      console.log(data);
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
      console.log(data);
    });

    // Reset Job dependant values for the player.
    this.http.resetPlayerJobDependantValues(this.player.id).subscribe((data) => {
      console.log(data);
    });

    this.player.staticRef.ConstructStaticPlayerInfo(this.player); // Reloads and recomputes all value

  }
}
