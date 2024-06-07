import { Gear } from "./gear";
let GEAR_LIST = ["Weapon", "Head", "Body","Hands",  "Legs", "Feet", "Earrings", "Necklace", "RightRing", "LeftRing", "Bracelet"];
export class Player {
  public id: number;
  public playerGearScore: number;
  public name: string;
  public job: number;
  public locked: boolean;
  public staticId: number;
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

  constructor(
  ){}

  public static CreatePlayerFromDict(Dict) : Player{
    let p = new Player();
    p.id = Dict["id"];
    p.job = Dict["job"];
    p.locked = Dict["locked"];
    p.name = Dict["name"];
    p.playerGearScore = Dict["playeGearScore"];

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
      for (let d in Dict["gearOptionPerGearType"][key]){
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
            break;
          case "RightRing":
            p.RightRingChoice.push(Gear.GearFromDict(d));
            break;
          case "LeftRing":
            p.LeftRingChoice.push(Gear.GearFromDict(d));
            break;
        }
      }
      break;
    }

    return p;
  }
}
