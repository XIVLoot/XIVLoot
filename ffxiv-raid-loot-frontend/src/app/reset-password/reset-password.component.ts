import { Component } from '@angular/core';
import { HttpService } from '../service/http.service';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent {

  public newPassword: string = "";
  public email: string = "";
  public token: string = "";
  constructor(public http: HttpService, private route: ActivatedRoute, public dialog: MatDialog,
    private _snackBar: MatSnackBar
) { } // Constructor with dependency injection

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.email = params['email'];
      this.token = params['token'];
    });
  }

  resetPassword() {
    this.http.ResetPassword(this.email, this.token, this.newPassword).subscribe((res: any) => {
      console.log(res);
    });
  }

}
