import { Component, OnInit } from '@angular/core';
import { Static } from '../models/static';
import { HttpService } from '../service/http.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-static-detail',
  templateUrl: './static-detail.component.html',
  styleUrls: ['./static-detail.component.css']
})
export class StaticDetailComponent implements OnInit {
  public staticDetail: Static;
  uuid: string;

  constructor(public http: HttpService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.uuid = params['uuid'];
    });
    this.http.getStatic(this.uuid).subscribe(details => {
      this.staticDetail = details;
      console.log(this.staticDetail);
    });
  }
}

