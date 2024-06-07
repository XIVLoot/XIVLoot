export class Gear {
  constructor(
    public id: number,
    public gearName: string,
    public gearItemLevel: number,
    public gearStage: number,
    public gearType: number,
    public gearCategory: number,
    public gearWeaponCategory: number
  ){}

  public static GearFromDict(Dict){
    return new Gear(Dict["id"], Dict["gearName"], Dict["gearItemLevel"], Dict["gearStage"], 0,0,0);
  }
}
