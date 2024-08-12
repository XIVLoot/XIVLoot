import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AbAuthComponent } from './about/about.component';
import { StaticComponent } from './static/static.component';
import { StaticDetailComponent } from './static-detail/static-detail.component';
import { AuthComponent } from './auth/auth.component';
import { CreateStaticComponent } from './create-static/create-static.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'about', component: AbAuthComponent },
  { path: 'create', component: CreateStaticComponent },
  { path : 'auth/discord/callback', component: AuthComponent},
  { path : 'reset-password', component: ResetPasswordComponent},
  //{ path: 'static', component: StaticComponent},
  { path: ':uuid', component: StaticDetailComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
