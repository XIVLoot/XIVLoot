import { Component } from '@angular/core';
import { MatMenuModule } from '@angular/material/menu';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  title = 'Loot Management';
  isLoggedIn = false;  // Add this line

  openLoginDialog(){
    window.open("http://localhost:4200/auth/discord/callback", "_blank");
  }
}
