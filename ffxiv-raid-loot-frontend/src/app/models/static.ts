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
    console.log("Hey");
    console.log(playersInfoList);
    for (let key in playersInfoList){
      let player = playersInfoList[key];
      let p = Player.CreatePlayerFromDict(player);
      console.log(p.name);
      this.players.push(p);
    }
  }
}
