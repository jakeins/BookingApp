import {CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, Route} from '@angular/router';
import { Injectable } from '@angular/core';

@Injectable()
export class AdminGuard implements CanActivate {
    constructor(private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {

      let role = true;

      if (role == true) {
        return true;
      } else {
        this.router.navigate(['/login']);
      }

    }

    canLoad(route: Route): boolean {
        let role = true;

        if (role == true) {
            return true;
        } else {
            this.router.navigate(['/login']);
        }
    }

}
