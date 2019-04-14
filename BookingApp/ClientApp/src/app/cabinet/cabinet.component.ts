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
        if (!this.userInfoService.isUser) {
          this.router.navigate(['/']);
        }
    });
    let userId = this.userInfoService.userId;
    this.id = userId;
   
    }

    ngOnDestroy() {
      this.authChangedSubscription.unsubscribe();
    };

}
