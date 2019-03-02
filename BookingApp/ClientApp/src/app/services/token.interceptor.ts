import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Logger } from './logger.service';
import { AccessTokenService } from './access-token.service';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  constructor(private aTokenService: AccessTokenService) { }
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    var accessToken = this.aTokenService.readToken();

    if (accessToken != undefined) {

      request = request.clone({
        setHeaders: {
          Authorization: 'Bearer ' + accessToken
        }
      });
    }

    return next.handle(request);
  }
}
