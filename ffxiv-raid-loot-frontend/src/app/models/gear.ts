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
    if (Dict === null)
      return new Gear(0, "No Equipment", 0, "Preparation", "", 0, 0);
    return new Gear(Dict["gearId"], Dict["gearName"], Dict["gearItemLevel"], Dict["gearStage"], "",0,0);
  }
}
