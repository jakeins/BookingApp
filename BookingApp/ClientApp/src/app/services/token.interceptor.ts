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

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  private isRefreshProccess = false;

  constructor(private tokenService: TokenService, private authService: AuthService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const jwt = this.tokenService.readJwtToken();
    let accessToken = null;
    let expireOn = null;
    if (jwt != null) {
      accessToken = jwt.accessToken;
      expireOn = jwt.expireOn;
    }

    if (!request.url.includes('logout') && accessToken != null) {
      if (expireOn < new Date() && !this.isRefreshProccess) {
        this.isRefreshProccess = true;
        this.tokenService.TokenExpired.emit();
      } else {
        this.isRefreshProccess = false;
      }

      request = request.clone({
        setHeaders: {
          Authorization: 'Bearer ' + accessToken
        }
      });
    }

    return next.handle(request).catch(error => {
      if (!request.url.includes('logout') && error.status === 401)
          this.authService.logout();
      
      return Observable.throw(error);
    });
  }
}
