import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about.component';
import { StaticComponent } from './static/static.component';
import { StaticDetailComponent } from './static-detail/static-detail.component';
import { OutComponent } from './out/out.component';

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'about', component: AboutComponent },
  { path: 'static', component: StaticComponent},
  { path: ':uuid', component: StaticDetailComponent },
  { path : 'auth/discord/callback', component: OutComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
