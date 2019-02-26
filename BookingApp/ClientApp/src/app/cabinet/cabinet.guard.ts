import {CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, Route, CanLoad} from '@angular/router';
import { Injectable } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Injectable()
export class CabinetGuard implements CanActivate, CanLoad {
  constructor(private router: Router, private authServ: AuthService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (this.authServ.isUser) {
        return true;
    }
    this.router.navigate(['/login']);
  }

  canLoad(route: Route): boolean {
    if (this.authServ.isUser) {
        return true;
    }
    this.router.navigate(['/login']);
  }

}
