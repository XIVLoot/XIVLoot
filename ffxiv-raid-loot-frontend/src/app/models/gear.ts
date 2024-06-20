export class Gear {
  constructor(
    public id: number,
    public gearName: string,
    public gearItemLevel: number,
    public gearStage: string,
    public gearType: string,
    public gearCategory: number,
    public gearWeaponCategory: number
  ){}

  public static GearFromDict(Dict){
    return new Gear(Dict["gearId"], Dict["gearName"], Dict["gearItemLevel"], Dict["gearStage"], "",0,0);
  }
}
