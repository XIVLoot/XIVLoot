import { Gear } from "./gear";
import { Static } from "./static";
let GEAR_LIST = ["Weapon", "Head", "Body","Hands",  "Legs", "Feet", "Earrings", "Necklace", "RightRing", "LeftRing", "Bracelet"];
export class Player {
  public staticRef : Static;
  public id: number;
  public playerGearScore: number;
  public name: string;
  public job: string;
  public LockedList : Date[];
  public staticId: number;
  public TomestoneCost : number;
  public TwineCost : number;
  public ShineCost : number;
  public CurrentAverageItemLevel : number;
  public BisAverageItemLevel : number;
  public etroBiS: string;
  public bisWeaponGear: Gear;
  public bisHeadGear: Gear;
  public bisBodyGear: Gear;
  public bisHandsGear: Gear;
  public bisLegsGear: Gear;
  public bisFeetGear: Gear;
  public bisEarringsGear: Gear;
  public bisNecklaceGear: Gear;
  public bisBraceletsGear: Gear;
  public bisLeftRingGear: Gear;
  public bisRightRingGear: Gear;
  public curWeaponGear: Gear;
  public curHeadGear: Gear;
  public curBodyGear: Gear;
  public curHandsGear: Gear;
  public curLegsGear: Gear;
  public curFeetGear: Gear;
  public curEarringsGear: Gear;
  public curNecklaceGear: Gear;
  public curBraceletsGear: Gear;
  public curLeftRingGear: Gear;
  public curRightRingGear: Gear;
  public WeaponChoice : Gear[] = [];
  public HeadChoice : Gear[] = [];
  public HandsChoice : Gear[] = [];
  public BodyChoice : Gear[] = [];
  public LegsChoice : Gear[] = [];
  public FeetChoice : Gear[] = [];
  public EarringsChoice : Gear[] = [];
  public NecklaceChoice : Gear[] = [];
  public BraceletsChoice : Gear[] = [];
  public RightRingChoice : Gear[] = [];
  public LeftRingChoice : Gear[] = [];
  public PGSGroupNumber : number = -1;
  public PGSGroupColor : string = 'rgba(255, 247, 0, 1)';
  public IsClaimed : boolean;
  public IsAlt : boolean;
  constructor(
  ){}

  public IsLockedOutOfTurn(turn : number) : boolean{
    return this.LockedList[turn-1] > new Date();
  }

  public static CreatePlayerFromDict(Dict) : Player{
    let p = new Player();
    p.id = Dict["id"];
    p.IsClaimed = Dict["isClaimed"];
    p.job = Dict["job"];
    p.LockedList = Dict["lockedList"].map(dateStr => new Date(dateStr));
    p.name = Dict["name"];
    p.playerGearScore = Dict["playerGearScore"];
    p.etroBiS=Dict["etroBiS"]; // TODO HAVE TO MAKE THIS WORK
    p.BisAverageItemLevel = Dict["averageItemLevelBis"];
    p.CurrentAverageItemLevel = Dict["averageItemLevelCurrent"];
    p.TomestoneCost = Dict["cost"]["tomeCost"];
    p.TwineCost = Dict["cost"]["twineCost"];
    p.ShineCost = Dict["cost"]["shineCost"];
    p.IsAlt = Dict["isAlt"];

    for (let key in Dict["bisGearSet"]){
      let d = Dict["bisGearSet"][key];
      switch (key){
        case "Weapon":
          p.bisWeaponGear = Gear.GearFromDict(d);
          break;
        case "Head":
          p.bisHeadGear = Gear.GearFromDict(d);
          break;
        case "Body":
          p.bisBodyGear = Gear.GearFromDict(d);
          break;
        case "Hands":
          p.bisHandsGear = Gear.GearFromDict(d)
          break;
        case "Legs":
          p.bisLegsGear = Gear.GearFromDict(d)
          break;
        case "Feet":
          p.bisFeetGear = Gear.GearFromDict(d)
          break;
        case "Necklace":
          p.bisNecklaceGear = Gear.GearFromDict(d)
          break;
        case "Earrings":
          p.bisEarringsGear = Gear.GearFromDict(d)
          break;
        case "Bracelets":
          p.bisBraceletsGear = Gear.GearFromDict(d)
          break;
        case "RightRing":
          p.bisRightRingGear = Gear.GearFromDict(d)
          break;
        case "LeftRing":
          p.bisLeftRingGear = Gear.GearFromDict(d)
          break;
      }
    }

    for (let key in Dict["currentGearSet"]){
      let d = Dict["currentGearSet"][key];
      switch (key){
        case "Weapon":
          p.curWeaponGear = Gear.GearFromDict(d);
          break;
        case "Head":
          p.curHeadGear = Gear.GearFromDict(d);
          break;
        case "Body":
          p.curBodyGear = Gear.GearFromDict(d);
          break;
        case "Hands":
          p.curHandsGear = Gear.GearFromDict(d);
          break;
        case "Legs":
          p.curLegsGear = Gear.GearFromDict(d);
          break;
        case "Feet":
          p.curFeetGear = Gear.GearFromDict(d);
          break;
        case "Necklace":
          p.curNecklaceGear = Gear.GearFromDict(d);
          break;
        case "Earrings":
          p.curEarringsGear = Gear.GearFromDict(d);
          break;
        case "Bracelets":
          p.curBraceletsGear = Gear.GearFromDict(d);
          break;
        case "RightRing":
          p.curRightRingGear = Gear.GearFromDict(d);
          break;
        case "LeftRing":
          p.curLeftRingGear = Gear.GearFromDict(d);
          break;
      }
    }

    for (let key in Dict["gearOptionPerGearType"]){
      for (let id in Dict["gearOptionPerGearType"][key]["gearOptionList"]){
        let d = Dict["gearOptionPerGearType"][key]["gearOptionList"][id];
        switch (key){
          case "Weapon":
            p.WeaponChoice.push(Gear.GearFromDict(d));
            break;
          case "Head":
            p.HeadChoice.push(Gear.GearFromDict(d));
            break;
          case "Hands":
            p.HandsChoice.push(Gear.GearFromDict(d));
            break;
          case "Body":
            p.BodyChoice.push(Gear.GearFromDict(d));
            break;
          case "Legs":
            p.LegsChoice.push(Gear.GearFromDict(d));
            break;
          case "Feet":
            p.FeetChoice.push(Gear.GearFromDict(d));
            break;
          case "Necklace":
            p.NecklaceChoice.push(Gear.GearFromDict(d));
            break;
          case "Earrings":
            p.EarringsChoice.push(Gear.GearFromDict(d));
            break;
          case "Bracelets":
            p.BraceletsChoice.push(Gear.GearFromDict(d));
            console.log(p.BraceletsChoice);
            break;
          case "RightRing":
            p.RightRingChoice.push(Gear.GearFromDict(d));
            break;
          case "LeftRing":
            p.LeftRingChoice.push(Gear.GearFromDict(d));
            break;
        }
      }
    }

    return p;
  }
  
  GetGroupColorNoAlpha(){
    switch(this.PGSGroupNumber){
      case 0:
        return'rgba(255, 247, 0, 1)';
      case 1:
        return'rgba(200, 0, 255, 1)';
      case 2:
        return'rgba(0, 21, 255, 1)';
      case 3:
        return'rgba(38, 255, 0, 1)';
      case -1:
        return'rgba(0, 0, 0, 0.3)';
    }
  }
}
