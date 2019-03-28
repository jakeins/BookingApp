import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { UserInfoService } from '../services/user-info.service';

@Component({
    selector: 'app-cabinet',
    templateUrl: './cabinet.component.html'
})

export class CabinetComponent implements OnInit {

  authChangedSubscription: any;
  name: string;

  constructor(
    private authService: AuthService,
    private router: Router,
    private userInfo: UserInfoService
  ) { }
    
    ngOnInit() {
      this.authChangedSubscription = this.authService.AuthChanged.subscribe(() => {
        if (!this.authService.isUser) {
          this.router.navigate(['/error/401']);
        }
      });

      this.name = this.userInfo.username;
    }

    ngOnDestroy() {
      this.authChangedSubscription.unsubscribe();
    };

}
