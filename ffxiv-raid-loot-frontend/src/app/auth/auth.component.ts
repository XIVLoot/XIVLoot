import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { HttpService } from '../service/http.service';
import { environment } from '../../environments/environments';
import { CLIENT_SECRET } from './secret';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.css'
})
export class AuthComponent {

  constructor(private http: HttpClient, private httpService: HttpService){

  }

  private client_id = "1252372424138166343";
  private client_secret = CLIENT_SECRET;

  toDiscord(){
    var url = environment.discord_redirect_url;
    
    // Check if the referrer is not the Discord login page
    if (!document.referrer.includes("discord.com")) {
      window.location.href = url;
      console.log("Redirecting to discord login page");
    }
    return false;
  }
  ngOnInit() {
    this.handleAuthentication();
  }

  handleAuthentication() {
    const hash = window.location.hash;
    const code = new URLSearchParams(window.location.search).get('code');
    if (code) {
      console.log('code:', code);
      const tokenUrl = 'https://discord.com/api/oauth2/token';
      const body = {};
      body['client_id'] = this.client_id;
      body['client_secret'] = this.client_secret; 
      body['grant_type'] = 'authorization_code';
      body['code'] = code;
      body['redirect_uri'] = environment.site_url + 'auth/discord/callback';
      console.log("Doing request");
      this.httpService.GetDiscordToken(body).subscribe(response => {
        //console.log('Access Token:', response);
        console.log("got a");
        // Store the access token securely
        //localStorage.setItem('discord_access_token_xiv_loot', response['access_token']);

        this.httpService.GetDiscordCookie(response['access_token']).subscribe(res => {
          console.log("Got token");
          var rurl = localStorage.getItem('return_url');
          localStorage.removeItem('return_url');
          window.location.href = rurl;
        });
        return;
      }, error => {
        console.error('Error fetching access token:', error);
      });
      }
    else{
      var url = environment.discord_redirect_url;
      window.location.href = url;
    }
  }
}
