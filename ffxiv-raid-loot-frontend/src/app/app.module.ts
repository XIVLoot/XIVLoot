import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { AbAuthComponent } from './about/about.component';
import { NavbarComponent } from './navbar/navbar.component';
import { StaticComponent } from './static/static.component';
import { SettingPGS, StaticDetailComponent } from './static-detail/static-detail.component';
import { HttpClientModule } from '@angular/common/http';

// Angular Material Modules
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatSidenavModule } from '@angular/material/sidenav';
import {FormControl, ReactiveFormsModule} from '@angular/forms';
import {MatSelectModule} from '@angular/material/select';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {Component} from '@angular/core';
import {MatDividerModule} from '@angular/material/divider';
import {
  MatDialog,
  MatDialogRef,
  MatDialogActions,
  MatDialogClose,
  MatDialogTitle,
  MatDialogContent,
} from '@angular/material/dialog';
import {
  MatSlideToggleModule,
  _MatSlideToggleRequiredValidatorModule,
} from '@angular/material/slide-toggle';


// Animation Provider
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { PlayerDetailComponent } from './player-detail/player-detail.component';
import { AuthComponent } from './auth/auth.component';
import { PlayerDetailsSingleComponent } from './player-details-single/player-details-single.component';
import { GearAcqHistorySingleComponent } from './gear-acq-history-single/gear-acq-history-single.component';
import { ItemBreakdownComponent } from './item-breakdown/item-breakdown.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AbAuthComponent,
    NavbarComponent,
    StaticComponent,
    StaticDetailComponent,
    PlayerDetailComponent,
    AuthComponent,
    PlayerDetailsSingleComponent,
    GearAcqHistorySingleComponent,
    ItemBreakdownComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    MatMenuModule,
    MatButtonModule,
    MatToolbarModule,
    MatExpansionModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    HttpClientModule,
    MatCardModule,
    MatListModule,
    MatGridListModule,
    MatSidenavModule,
    MatCheckboxModule,
    MatSelectModule,
    MatDividerModule,
    MatDialogActions, 
    MatDialogClose, 
    MatDialogTitle, 
    MatDialogContent,
    MatSlideToggleModule,
    MatTooltipModule
  ],
  providers: [
    provideAnimationsAsync()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
