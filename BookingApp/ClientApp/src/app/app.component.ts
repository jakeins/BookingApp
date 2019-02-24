import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']

})
export class AppComponent implements OnInit {

  constructor(private authService: AuthService) {}
  flag: boolean = false;
  admin: boolean = false;
  user: boolean = false;

  ngOnInit() {
    this.checkAuth();
  }

  
  authAdmin() {
    let login = "superadmin@admin.cow";
    let password = "SuperAdmin";
    this.authService.login(login, password)
        .subscribe(res => {
          if (res.accessToken != null) {
            this.authService.setToken(res.accessToken);
            this.flag = true;
            this.admin = true;
          }
        });
  }

  logout() {
    this.authService.removeToken();
    this.flag = false;
    this.admin = false;
    this.user = false;
  }

  authUser() {
    let login = "lion@user.cow";
    let password = "Lion";
    this.authService.login(login, password)
      .subscribe(res => {
        if (res.accessToken != null) {
          this.authService.setToken(res.accessToken);
          this.flag = true;
          this.user = true;
        }
      });
  }

  checkAuth() {
    if (this.authService.checkAuth() !== null) {
      let roleName = this.authService.checkAuth();
      if (roleName == "Lion") {
        this.flag = true;
        this.admin = false;
        this.user = true;
      } else if (roleName == "SuperAdmin") {
        this.flag = true;
        this.admin = true;
        this.user = false;
      }
    }
  }

}
