import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { StaticGroupsComponent } from './static-groups/static-groups.component';
import Approutes from './app.routes';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    AppComponent,
    StaticGroupsComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    RouterModule.forRoot(Approutes), // Set up routing
    CommonModule

  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }