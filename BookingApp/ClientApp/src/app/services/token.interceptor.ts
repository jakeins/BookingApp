import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/throw';

import { Logger } from './logger.service';
import { TokenService } from './token.service';
import { AuthService } from './auth.service';
import { map } from 'rxjs/operator/map';
import { Router } from '@angular/router';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  private refreshIsOngoing = false;
  private refreshLock = false;
  private failedRequests = 0;
  

  constructor(private tokenService: TokenService, private authService: AuthService, private router: Router) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    //get saved token info
    const jwt = this.tokenService.readJwtToken();
    let accessToken = null;
    let expireOn = null;
    if (jwt != null) {
      accessToken = jwt.accessToken;
      expireOn = jwt.expireOn;
    }

    const isLogout: boolean = request.url.indexOf('auth/logout') !== -1;
    const isRefresh: boolean = request.url.indexOf('auth/refresh') !== -1;
    
    //positive authentication intervention
    if (!isLogout && !isRefresh && accessToken != null) {
      this.refreshLock = true;
      const alreadyExpired: boolean = expireOn <= new Date();
      const timeToRefresh: boolean = Date.parse(expireOn).valueOf() - new Date().valueOf() < 1 * 60 * 60 * 1000;
      if (timeToRefresh) {
        Logger.warn(`Token interceptor: access token expired on [${request.url}]. Request may fail.`);
        if (this.refreshIsOngoing === false) {
          Logger.warn(`Token interceptor: requesting refresh from [${request.url}].`);
          this.refreshIsOngoing = true;
          this.authService.refresh().subscribe(() => {
            this.refreshIsOngoing = false;
            Logger.warn(`Token interceptor: finishing refresh from [${request.url}].`);
            if (this.failedRequests > 0) {
              this.failedRequests = 0;
              this.router.navigate(['']);
            }

          }, error => {
            Logger.error(`Token interceptor: refresh from [${request.url}] gave an error. Logging out.`);
            Logger.error(error);
            this.authService.logout();
          });
        }
        else {
          Logger.warn(`Token interceptor: refresh from [${request.url}] is ignored because it is already in progress.`);
        }
        this.refreshLock = false;
      }
      request = request.clone({
        setHeaders: {
          Authorization: 'Bearer ' + accessToken
        }
      });      
    }    

    return next.handle(request).catch(error => {
      if (!isLogout && error.status === 401) {
        if (this.refreshIsOngoing)
          this.failedRequests++;
        else {
          this.authService.logout();
          Logger.error(`Token interceptor: unathorized during [${request.url}], not waiting for refresh. Logging out.`);
        }
          
      }
      return Observable.throw(error);
    });
  }
}
