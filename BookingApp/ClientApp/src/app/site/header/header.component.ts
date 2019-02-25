import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class AppHeaderComponent implements OnInit {

  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.resetAuthFlags();
  }

  resetAuthFlags() {
    this.authService.resetAuthFlags();
  }

  authAdmin() {
    this.authService.authAsAdmin();
  }

  authUser() {
    this.authService.authAsUser();
  }

  logout() {
    this.authService.logout();
  }

}
