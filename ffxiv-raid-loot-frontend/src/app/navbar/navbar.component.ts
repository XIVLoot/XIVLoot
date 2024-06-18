import { Component } from '@angular/core';
import { MatMenuModule } from '@angular/material/menu';
import { HttpService } from '../service/http.service';
import { PizzaPartyAnnotatedComponent } from '../static-detail/static-detail.component';
import { MatSnackBar } from '@angular/material/snack-bar';

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

  constructor(private http : HttpService, private _snackBar: MatSnackBar){}

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
    localStorage.setItem('return_url', window.location.href);
    window.open("http://localhost:4200/auth/discord/callback", "_blank");
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
