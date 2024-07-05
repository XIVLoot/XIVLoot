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
      const body = new URLSearchParams();
      body.set('client_id', this.client_id);
      body.set('client_secret', this.client_secret); 
      body.set('grant_type', 'authorization_code');
      body.set('code', code);
      body.set('redirect_uri', environment.site_url + 'auth/discord/callback');
  
      this.http.post(tokenUrl, body.toString(), {
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
      }).subscribe(response => {
        console.log('Access Token:', response);
        // Store the access token securely
        //localStorage.setItem('discord_access_token_xiv_loot', response['access_token']);

        this.httpService.GetDiscordCookie(response['access_token']).subscribe(res => {
          console.log(res);
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
