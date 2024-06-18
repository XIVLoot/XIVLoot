import { Player } from './player';
export class Static {
  public players : Player[] = [];
  constructor(
    public id: number,
    public name: string,
    public uuid: string,
    playersInfoList
  ){
    this.uuid = uuid;
    this.name = name;
    this.id = id;
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
