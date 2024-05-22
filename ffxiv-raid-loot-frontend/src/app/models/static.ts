import { Player } from './player';
export class Static {
  constructor(
    public id: number,
    public name: string,
    public uuid: string,
    public players: Player[]
  ){}
}
