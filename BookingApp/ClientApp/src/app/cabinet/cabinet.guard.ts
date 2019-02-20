import {CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, Route, CanLoad} from '@angular/router';
import { Injectable } from '@angular/core';

@Injectable()
export class CabinetGuard implements CanActivate, CanLoad {
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
