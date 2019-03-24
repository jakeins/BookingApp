import { Logger } from './logger.service';
import { JwtToken } from '../models/jwt-token';
import { Injectable, EventEmitter, Output } from '@angular/core';

@Injectable()
export class TokenService {

  private accessTokenName = 'accessToken';
  private refreshTokenName = 'refreshToken';
  private accessTokenExpName = 'accessTokenExpires';

  @Output() TokenExpired: EventEmitter<any> = new EventEmitter();

  public writeToken(jwtToken: JwtToken): void {
    localStorage.setItem(this.accessTokenName, jwtToken.accessToken);
    localStorage.setItem(this.refreshTokenName, jwtToken.refreshToken);
    localStorage.setItem(this.accessTokenExpName, jwtToken.expireOn.toString() );
  }

  public readJwtToken(): JwtToken {
    const accessToken = localStorage.getItem(this.accessTokenName);
    const refreshToken = localStorage.getItem(this.refreshTokenName);
    let token: JwtToken = null;

    if (accessToken != null && refreshToken != null) {
      token = {
        accessToken: accessToken,
        refreshToken: refreshToken,
        expireOn: new Date(localStorage.getItem(this.accessTokenExpName))
      };
    }

    return token;
  }

  public deleteToken(): void {
    localStorage.removeItem(this.accessTokenName);
    localStorage.removeItem(this.refreshTokenName);
    localStorage.removeItem(this.accessTokenExpName);
  }
}
