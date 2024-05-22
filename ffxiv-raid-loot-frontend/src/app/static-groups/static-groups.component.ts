import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-static-groups',
  templateUrl: './static-groups.component.html',
  styleUrls: ['./static-groups.component.css'],
  imports: [CommonModule],
  standalone: true  // Mark the component as standalone
})
export class StaticGroupsComponent implements OnInit {
  groups: any = [];

  constructor() { }

  ngOnInit(): void {
    // Initialization logic here
  }
}