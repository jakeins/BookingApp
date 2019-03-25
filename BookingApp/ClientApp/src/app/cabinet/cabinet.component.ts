import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { UserInfoService } from '../services/user-info.service';

@Component({
    selector: 'app-cabinet',
    templateUrl: './cabinet.component.html'
})
export class CabinetComponent implements OnInit {

  authChangedSubscription: any;
  id:string

  constructor(private authService: AuthService, private router: Router, private userService: UserService, private userInfoService: UserInfoService) { }
    
  ngOnInit() {
      this.authChangedSubscription = this.authService.AuthChanged.subscribe(() => {
        if (!this.authService.isUser) {
          this.router.navigate(['/error/401']);
        }
    });
    let userName = this.userInfoService.username;
    this.userService.getUserByUserName(userName).subscribe((res) => {
      this.id = res.id;
    });
    }

    ngOnDestroy() {
      this.authChangedSubscription.unsubscribe();
    };

}
