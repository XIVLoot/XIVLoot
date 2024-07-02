import { Component } from '@angular/core';
import { MatMenuModule } from '@angular/material/menu';
import { HttpService } from '../service/http.service';
import { PizzaPartyAnnotatedComponent } from '../static-detail/static-detail.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from '../../environments/environments';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
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
  public isLoggedInDiscord = false;
  public isLoggedInDefault = false;
  public discordInfo : any = {};
  public userSavedStatic = [];

  public username = "";

  constructor(private http : HttpService, private _snackBar: MatSnackBar, private _dialog : MatDialog){}

  ngOnInit(){
    try{
      this.http.GetUsernameDefault()
      .subscribe(
        (res : string) => {
          this.username = res;
          this.isLoggedInDefault = true;
          this.isLoggedInDiscord = false;
          this.isLoggedIn = true;
          localStorage.removeItem('discord_access_token_xiv_loot');
          this.getUserSavedStatic();
          
        }
      );
    } catch(error){
      this.isLoggedInDefault = false;
      this.isLoggedInDiscord = false;
    }

    this.isLoggedInDiscord = localStorage.getItem('discord_access_token_xiv_loot') !== null;

    if (this.isLoggedInDiscord){
      // Retrieving discord info
      try{
        this.http.getDiscorduserInfo(localStorage.getItem('discord_access_token_xiv_loot')!).
        subscribe((data) => {
          this.discordInfo = data;
          this.username = this.discordInfo.global_name;
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
    
    this.isLoggedIn = false; // Update isLoggedIn status
    this.isLoggedInDiscord = false;
    localStorage.removeItem('discord_access_token_xiv_loot');
    this.http.Logout().subscribe(res => {
      this.isLoggedInDefault=false
      window.location.reload();
  });
    
  }

  openLoginDialog(){
    this._dialog.open(LoginDialog, {height:'530px',width:'500px'}).afterClosed().subscribe(res => {
      this.isLoggedInDefault = res;
      this.ngOnInit();


    });
    //localStorage.setItem('return_url', window.location.href);
    //window.open(environment.site_url + "auth/discord/callback", "_blank");
  }

  get discordAvatarUrl(): string {
    if (this.isLoggedInDiscord) {
        return `https://cdn.discordapp.com/avatars/${this.discordInfo["id"]}/${this.discordInfo["avatar"]}.png`;
    } else {
      return 'assets/t.png';  // Path to a default image
    }
  }

  removeStatic(uuid : string){
    if (this.isLoggedInDiscord){
      this.http.RemoveUserSavedStaticDiscord(this.discordInfo["id"], uuid).subscribe((res : string) => {
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
    } else if (this.isLoggedInDefault){
      this.http.RemoveUserSavedStaticDefault(uuid).subscribe((res : string) => {
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
  }

  GetStaticLink(){
    return environment.site_url;
  }

  getUserSavedStatic(){
    if (this.isLoggedInDiscord){
      if(this.discordInfo["id"] !== undefined){
        this.http.GetUserSavedStaticDiscord(this.discordInfo["id"]).subscribe((res : string[])   => {
          for(let i = 0; i < res.length; i++){
            this.http.getStaticName(res[i]).subscribe((data) => {
              this.userSavedStatic.push({name : data, url : res[i]});
            });
          }
        });
      }
    }
    else if (this.isLoggedInDefault){
      this.http.GetUserSavedStaticDefault().subscribe((res : string[])   => {
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
  constructor(private _snackBar: MatSnackBar, private http : HttpService, private dialogRef: MatDialogRef<LoginDialog>) {}

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
        this.loginEmail = this.registerEmail;
        this.loginPassword = this.registerPassword;
        this.login(false);
      });
    });
  }

  async login(ShowSuccess : boolean){
    var check = await new Promise<boolean>(resolve => {this.http.Login(this.loginEmail, this.loginPassword).subscribe((res : any) => {
      if (ShowSuccess){
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 3500,
          data: {
            message: "Successfuly logged in.",
            subMessage: "",
            color : ""
          }
        });
      }
      resolve(true);
    }, (error : any) => {this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
      duration: 3500,
      data: {
        message: "Error while logging in.",
        subMessage: "",
        color : ""
      }
    });resolve(false);});
  });
  this.dialogRef.close(check); 
  
}
}
