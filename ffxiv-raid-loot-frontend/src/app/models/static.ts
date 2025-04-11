import { Player } from './player';
export class Static {
  public players : Player[] = [];
  public useBookForGearAcq : boolean = false;
  public LockParam = {    
  "BOOL_LOCK_PLAYERS": false,
  "BOOL_LOCK_IF_NOT_CONTESTED": false,
  "RESET_TIME_IN_WEEK": 1,
  "BOOL_FOR_1_FIGHT": true,
  "INT_NUMBER_OF_PIECES_UNTIL_LOCK": 1,
  "LOCK_IF_TOME_AUGMENT": false,
  "BOOL_IF_ROLE_CHANGES_NUMBER_PIECES": 0,
  "DPS_NUMBER": 0,
  "TANK_NUMBER": 0,
  "HEALER_NUMBER": 0};

  public test : boolean;
  public userOwn : any = {};
  constructor(
    public id: number,
    public name: string,
    public uuid: string,
    public Tier : number,
    playersInfoList,
    lockParam
  ){
    this.uuid = uuid;
    this.name = name;
    this.id = id;
    this.Tier = Tier
    this.test = false;
    this.userOwn = {};
    for (let key in lockParam){
      if (key.includes("BOOL") || key == "LOCK_IF_TOME_AUGMENT"){
        this.LockParam[key] = lockParam[key] == 0 ? false : true;
      }
      else{
        this.LockParam[key] = lockParam[key];
      }
    }


    this.ConstructStaticPlayerInfo(playersInfoList);
  }

  ConstructStaticPlayerInfo(playersInfoList){
    for (let key in playersInfoList){
      let player = playersInfoList[key];
      let p = Player.CreatePlayerFromDict(player);
      p.staticRef = this;
      this.players.push(p);
    }
  }
}
