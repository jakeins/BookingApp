import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, Route, CanLoad } from '@angular/router';
import { Injectable } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { UserInfoService } from '../services/user-info.service';

@Injectable()
export class CabinetGuard implements CanActivate, CanLoad {
  constructor(private router: Router, private userInfo: UserInfoService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (this.userInfo.isUser) {
        return true;
    }
    this.router.navigate(['/login']);
  }

  canLoad(route: Route): boolean {
    if (this.userInfo.isUser) {
        return true;
    }
    this.router.navigate(['/login']);
  }

}
