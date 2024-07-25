import { Component, Inject } from '@angular/core';
import { MatMenuModule } from '@angular/material/menu';
import { HttpService } from '../service/http.service';
import { PizzaPartyAnnotatedComponent } from '../static-detail/static-detail.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from '../../environments/environments';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButton } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { catchError, throwError } from 'rxjs';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {

  public hasStatic : boolean = false;
  public curUUID : string = "";
  public staticName : string = "";

  public title = 'Loot Management';
  public isLoggedIn = false;
  public isLoggedInDiscord = false;
  public isLoggedInDefault = false;
  public discordInfo : any = {};
  public userSavedStatic : any= [];

  public recentStatic : any= [];

  public username = "";

  constructor(private http : HttpService, private _snackBar: MatSnackBar, private _dialog : MatDialog){}

  async ngOnInit(){

    this.isLoggedInDiscord = await this.http.CheckAuthDiscord();
    if (this.isLoggedInDiscord){
      // Retrieving discord info
      try{
        this.http.GetDiscordUserInfo().subscribe(res => {
          this.discordInfo = res;
          this.username = this.discordInfo.global_name;
          this.getUserSavedStatic();
          this.isLoggedInDefault = false;
          this.isLoggedIn = true;
        });
      }
      catch(error){
        this.isLoggedIn = false;
        ////console.log(error.message);
        return;
      }
    }

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

    if (localStorage.getItem('recentStatic') !== null){
      var retrievedList = JSON.parse(localStorage.getItem('recentStatic') || '[]');
      for (let i in retrievedList){
        var uuid = retrievedList[i];
        this.http.getStaticName(uuid).subscribe(res => {
          this.recentStatic.push([uuid, res])
        });
      }
    }




  }

  // Method to check if URL contains a UUID
  containsUUID(url: string): boolean {
    const uuidRegex = /[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}/;
    return uuidRegex.test(url);
  }

  logout(){
    
    if(this.isLoggedInDiscord){
      this.http.LogoutDiscord().pipe(catchError(error => {
        if (error.error.text !== "Logged out successfully."){
          return throwError(() => new Error('Failed to logout discord: ' + error.error.text));
        }
        this.isLoggedIn = false;this.isLoggedInDiscord=false;window.location.reload();
      })).subscribe(res => {this.isLoggedIn = false;this.isLoggedInDiscord=false;window.location.reload();});
      return;
    }

    this.http.Logout().subscribe(res => {
      this.isLoggedInDefault=false
      this.isLoggedIn = false; // Update isLoggedIn status
      window.location.reload();
  });
    
  }

  openLoginDialog(){
    this._dialog.open(LoginDialog, {height:'640px',width:'500px'}).afterClosed().subscribe(res => {
      this.isLoggedInDefault = res === 1;
      this.isLoggedInDiscord = res === 2;
      if (this.isLoggedInDefault || this.isLoggedInDiscord){
        this.isLoggedIn = true;
        window.location.reload();
      }
    });
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
        ////console.log(res);
      this.userSavedStatic = this.userSavedStatic.filter((s) => s.url !== uuid);
      this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
        duration: 3500,
        data: {
          message: "Successfully removed static.",
          subMessage: "",
          color : "green"
        }
        });
      });
    } else if (this.isLoggedInDefault){
      this.http.RemoveUserSavedStaticDefault(uuid).subscribe((res : string) => {
        ////console.log(res);
        this.userSavedStatic = this.userSavedStatic.filter((s) => s.url !== uuid);
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 3500,
          data: {
            message: "Successfully removed static.",
            subMessage: "",
            color : "green"
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

  openAcountInfo(){
    this._dialog.open(ProfileDialog, {height:'530px',width:'500px', data:{username : this.username, isDiscord : this.isLoggedInDiscord}}).afterClosed().subscribe(res => {
    });
  }


}

@Component({
  selector: 'profile-dialog',
  templateUrl: `profile.dialog.html`,
  standalone: true,
  imports: [MatInputModule, MatFormFieldModule, MatButton, CommonModule, FormsModule],
})
export class ProfileDialog {
  constructor(private _snackBar: MatSnackBar, private http : HttpService, private dialogRef: MatDialogRef<ProfileDialog>,
    @Inject(MAT_DIALOG_DATA) public data: { username : string, email :string, isDiscord : boolean},
  ) {}

  

}

@Component({
  selector: 'login-dialog',
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
  public registerUsername : string = "";

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
          message: "Successfully registered.",
          subMessage: "",
          color : "green"
        }
      });
      this.http.Login(this.registerEmail, this.registerPassword).subscribe((res : any) => {
        this.loginEmail = this.registerEmail;
        this.loginPassword = this.registerPassword;
        this.login(false);
      });
    });
  }

  loginDiscord(){
    localStorage.setItem('return_url', window.location.href);
    window.open(environment.site_url + "auth/discord/callback", "_blank");
    this.dialogRef.close(2); 
  }



  async login(ShowSuccess : boolean){
    var check = await new Promise<boolean>(resolve => {this.http.Login(this.loginEmail, this.loginPassword).subscribe((res : any) => {
      if (ShowSuccess){
        this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
          duration: 3500,
          data: {
            message: "Successfully logged in.",
            subMessage: "",
            color : "green"
          }
        });
      }else {
        this.http.SetUsername(this.registerUsername).subscribe((res : any) => {
        ////console.log(res);
      });
}
      resolve(true);
    }, (error : any) => {this._snackBar.openFromComponent(PizzaPartyAnnotatedComponent, {
      duration: 3500,
      data: {
        message: "Error while trying to logging in.",
        subMessage: "(Reach out if this persists)",
        color : "red"
      }
    });resolve(false);});
  });
  this.dialogRef.close(check ? 1 : 0); 
  
}
}
