import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html'
})
export class AdminComponent implements OnInit {

  authChangedSubscription: any;

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit() {
    this.authChangedSubscription = this.authService.AuthChanged.subscribe(() => {
      if (!this.authService.isAdmin) {
        this.router.navigate(['/']);
      }
    });
  }

  ngOnDestroy() {
    this.authChangedSubscription.unsubscribe();
  };

}
