import {CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, Route} from '@angular/router';
import { Injectable } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AdminGuard implements CanActivate {
  constructor(private router: Router, private authServ: AuthService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (this.authServ.isAdmin) {
        return true;
    }
    this.router.navigate(['/login']);
  }

  canLoad(route: Route): boolean {
    if (this.authServ.isAdmin) {
        return true;
    }
    this.router.navigate(['/login']);
  }

}
