import { Component, Input } from '@angular/core';
import { DataService } from '../service/data.service';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { Static } from '../models/static';

@Component({
  selector: 'app-static',
  templateUrl: './static.component.html',
  styleUrl: './static.component.css'
})

export class StaticComponent {
  constructor(public http: HttpClient, public data: DataService){}
  staticName: string = '';

  ngOnInit(): void {

  }
  private api = 'https://localhost:7203/api/';

async AddStatic(name: string) {
  this.http.post(this.api + 'Static?name=' + name, {})
      .pipe(map(response => {
        let newStatic = new Static(response['id'], response['name'], response['uuid'], response['players']);
        return newStatic;
      }))
      .subscribe(response => {
        this.data.static = response;
        console.log(response);
      });
}

}
