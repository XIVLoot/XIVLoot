import { Component } from '@angular/core';
import { environment } from '../../environments/environments';
import { HttpService } from '../service/http.service';
import { DataService } from '../service/data.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { map } from 'rxjs';
import { HomeClaimPlayerToolTip } from '../tooltip';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  // Constructor with HttpClient and DataService injected
  constructor(public http: HttpService, public data: DataService, public router: Router, private _snackBar: MatSnackBar){}
  staticName: string = ''; // Property to store the name of a static

  public claimedPlayerList : any = [];
  public IsLoggedIn : boolean = false;
  private api = environment.api_url; // Base URL for the API
  private url = environment.site_url;
  public IsLoading = false;

  public HomeClaimPlayerToolTip = HomeClaimPlayerToolTip

  // Lifecycle hook that is called after Angular has initialized all data-bound properties
  async ngOnInit(): Promise<void> {
   await this.initClaimedPlayerList();
  }

  getBackgroundColor(job: string){
    switch(job){
      case "BlackMage":
      case "RedMage":
      case "Summoner":
      case "Ninja":
      case "Samurai":
      case "Monk":
      case "Reaper":
      case "Dragoon":
      case "Bard":
      case "Machinist":
      case "Dancer":
      case "Viper":
      case "Pictomancer":        
        return "rgba(255, 0, 0, 0.7)";
      case "Astrologian":
      case "Sage":
      case "Scholar":
      case "WhiteMage":
        return "rgba(0,255,0,0.7)";
      case "DarkKnight":
      case "Paladin":
      case "Warrior":
      case "Gunbreaker":
        return "rgba(0, 0, 255, 0.7)";
    }
  }

  EditPlayer(playerId : string, uuid : string){
    window.location.href = environment.site_url + "/" + uuid + "?pId=" + playerId;
  }


  async initClaimedPlayerList(){
    var DiscordLoggedIn = await this.http.CheckAuthDiscord();
    var DefaultLoggedIn;
    try {
      DefaultLoggedIn = await this.http.CheckAuthDefault();
    } catch (error) {
      DefaultLoggedIn = false;
    }
    if (!DefaultLoggedIn && !DiscordLoggedIn){
      return;
    }

    this.IsLoggedIn=true;
    this.IsLoading = true;
    
    if(DiscordLoggedIn){
      this.http.GetDiscordUserInfo().subscribe(data => {
        this.http.GetAllClaimedPlayerDiscord(data["id"]).subscribe(res => {this.claimedPlayerList = res;this.IsLoading = false;});
      });
    } else if (DefaultLoggedIn){
      this.http.GetAllClaimedPlayerDefault().subscribe(res => {this.claimedPlayerList = res;this.IsLoading = false;});
    }
  }


  // Asynchronous method to add a new static entity
  async AddStatic(name: string, Tier : number) {
    // Making a POST request to the API to add a new static
    this.http.AddStatic(name, Tier)
      .pipe(map(response => {
        // Mapping the response to a Static model
        //let newStatic = new Static(response['id'], response['name'], response['uuid'], response['players']);
        return response;
      }))
      .subscribe(response => {
        // Subscribing to the observable to handle the response
        this.data.static = response; // Storing the response in DataService
        ////console.log(response); // Logging the response to the console
        this.router.navigate(['/' + response]);
      });
  }

}