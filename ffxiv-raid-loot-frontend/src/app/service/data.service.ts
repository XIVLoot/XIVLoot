import { Injectable } from '@angular/core';
import { Static } from '../models/static';
import { Player } from '../models/player';
import { Gear } from '../models/gear';

@Injectable({
  providedIn: 'root'
})
export class DataService {

constructor() { }
  static: Static[];

  players: Player[];

  gearSet: Gear[];
}
