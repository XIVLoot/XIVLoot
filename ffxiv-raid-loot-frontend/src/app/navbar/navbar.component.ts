import { Component } from '@angular/core';
import { MatMenuModule } from '@angular/material/menu';
import { HttpService } from '../service/http.service';
import { PizzaPartyAnnotatedComponent } from '../static-detail/static-detail.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from '../../environments/environments';
import { MatDialog } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButton } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  public title = 'Loot Management';
  public isLoggedIn = false;
  public discordInfo = {
  }
  public userSavedStatic = [];

  constructor(private http : HttpService, private _snackBar: MatSnackBar, private _dialog : MatDialog){}

  ngOnInit(){
    this.isLoggedIn = localStorage.getItem('discord_access_token_xiv_loot') !== null;

    // Retrieving discord info

    if (this.isLoggedIn){
      try{
        this.http.getDiscorduserInfo(localStorage.getItem('discord_access_token_xiv_loot')!).
        subscribe((data) => {
          this.discordInfo = data;
          this.getUserSavedStatic();
        });
      }
      catch(error){
        this.isLoggedIn = false;
        console.log(error.message);
      }
    }

  }

  logout(){
    localStorage.removeItem('discord_access_token_xiv_loot');
    this.isLoggedIn = false; // Update isLoggedIn status
    window.location.reload();
  }

  openLoginDialog(){
    this._dialog.open(LoginDialog, {height:'530px',width:'500px'});
    //localStorage.setItem('return_url', window.location.href);
    //window.open(environment.site_url + "auth/discord/callback", "_blank");
  }

  get discordAvatarUrl(): string {
    if (this.isLoggedIn) {
      return `https://cdn.discordapp.com/avatars/${this.discordInfo["id"]}/${this.discordInfo["avatar"]}.png`;
    } else {
      return 'assets/t.png';  // Path to a default image
    }
  }

  removeStatic(uuid : string){
    this.http.RemoveUserSavedStatic(this.discordInfo["id"], uuid).subscribe((res : string) => {
      console.log(res);
      this.userSavedStatic = this.userSavedStatic.filter((s) => s.url !== uuid);
      this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
        duration: 3500,
        data: {
          message: "Successfuly removed static.",
          subMessage: "",
          color : ""
        }
      });
    });
  }

  GetStaticLink(){
    return environment.site_url;
  }

  getUserSavedStatic(){
    if(this.discordInfo["id"] !== undefined){
      this.http.GetUserSavedStatic(this.discordInfo["id"]).subscribe((res : string[])   => {
        for(let i = 0; i < res.length; i++){
          this.http.getStaticName(res[i]).subscribe((data) => {
            this.userSavedStatic.push({name : data, url : res[i]});
          });
        }
      });
    }
  }


}

@Component({
  selector: 'import-etro',
  templateUrl: `login.dialog.html`,
  standalone: true,
  imports: [MatInputModule, MatFormFieldModule, MatButton, CommonModule, FormsModule],
})
export class LoginDialog {
  constructor(private _snackBar: MatSnackBar, private http : HttpService) {}

  public showLogin: boolean = true;
  public loginEmail : string = "";
  public loginPassword : string = "";
  
  public registerEmail : string = "";
  public registerPassword : string = "";
  public confirmPassword : string = "";
  ngOnInit(){
    this.showLogin = true;
    this.loginEmail = "";
    this.loginPassword = "";
    
    this.registerEmail = "";
    this.registerPassword  = "";
    this.confirmPassword = "";
  }

  register(){
    if(this.registerPassword !== this.confirmPassword){
      this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
        duration: 3500,
        data: {
          message: "Passwords do not match.",
          subMessage: "",
          color : "red"
        }
      });
      return;
    }

    this.http.Register(this.registerEmail, this.registerPassword).subscribe((res : any) => {
      this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
        duration: 3500,
        data: {
          message: "Successfuly registered.",
          subMessage: "",
          color : ""
        }
      });
      this.http.Login(this.registerEmail, this.registerPassword).subscribe((res : any) => {
        console.log(res);
      });
    });
  }

  login(){
    this.http.Login(this.loginEmail, this.loginPassword).subscribe((res : any) => {
      console.log(res);
    });
  }
}
