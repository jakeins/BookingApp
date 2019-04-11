import { Logger } from './logger.service';
import { JwtToken } from '../models/jwt-token';
import * as jwt_decode from 'jwt-decode';
import { Injectable, EventEmitter, Output } from '@angular/core';
import { UserInfoService } from './user-info.service';

@Injectable()
export class TokenService {

  private accessTokenName = 'accessToken';
  private refreshTokenName = 'refreshToken';
  private accessTokenExpName = 'accessTokenExpires';
  

  private roleKey = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

  @Output() AccessTokenExpired: EventEmitter<any> = new EventEmitter();

  constructor(private userInfoService: UserInfoService) {
  }

  public writeToken(jwtToken: JwtToken): void {
    localStorage.setItem(this.accessTokenName, jwtToken.accessToken);
    localStorage.setItem(this.refreshTokenName, jwtToken.refreshToken);
    localStorage.setItem(this.accessTokenExpName, jwtToken.expireOn.toString());

    this.userInfoService.SaveUserInfo(jwt_decode(jwtToken.accessToken));
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

    this.userInfoService.DeleteUserInfo();
  }
}
