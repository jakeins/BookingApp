import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-cabinet',
    templateUrl: './cabinet.component.html'
})
export class CabinetComponent implements OnInit {

    authChangedSubscription: any;

    constructor(private authService: AuthService, private router: Router) { }
    
    ngOnInit() {
      this.authChangedSubscription = this.authService.AuthChanged.subscribe(() => {
        if (!this.authService.isUser) {
          this.router.navigate(['/error/401']);
        }
      });
    }

    ngOnDestroy() {
      this.authChangedSubscription.unsubscribe();
    };

}
