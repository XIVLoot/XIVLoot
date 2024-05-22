import { Component, Input } from '@angular/core';
import { HttpService } from '../service/http.service';

@Component({
  selector: 'app-static',
  templateUrl: './static.component.html',
  styleUrl: './static.component.css'
})

export class StaticComponent {
  constructor(public http: HttpService){}
  staticName: string = '';

  ngOnInit(): void {

  }

AddStatic(name: string) {
  this.http.addStatic(name);
}

}
