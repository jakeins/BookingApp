import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { UserInfoService } from '../services/user-info.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html'
})
export class AdminComponent implements OnInit {

  authChangedSubscription: any;

  constructor(private userInfo: UserInfoService, private router: Router, private authService: AuthService) { }

  ngOnInit() {
    this.authChangedSubscription = this.authService.AuthChanged.subscribe(() => {
      if (!this.userInfo.isAdmin) {
        this.router.navigate(['/']);
      }
    });
  }

  ngOnDestroy() {
    this.authChangedSubscription.unsubscribe();
  };

}
