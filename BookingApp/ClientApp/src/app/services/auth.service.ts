import { Injectable, Output, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Logger } from './logger.service';
import { JwtToken } from '../models/jwt-token';
import { BASE_API_URL } from '../globals';
import { TokenService } from './token.service';
import { UserInfoService } from './user-info.service';
import { RegisterFormModel } from '../models/register-form.model';

@Injectable()
export class AuthService {
  private roleAdminCache?: boolean = null;
  private roleUserCache?: boolean = null;
  private baseUrlRegister: string;
  private baseUrlLogin: string;
  private baseUrlLogout: string;
  private baseUrlRefresh: string;
  private baseUrlForget: string;
  private headers = new HttpHeaders({
    'Content-Type': 'application/json', 'Accept': 'application/json'
  });
  @Output() AuthChanged: EventEmitter<any> = new EventEmitter();

  public get isAdmin(): boolean {
    this.fillRoles();
    return this.roleAdminCache === true;
  }

  public get isUser(): boolean {
    this.fillRoles();
    return this.roleUserCache === true;
  }

  public get isAnon(): boolean {
    this.fillRoles();
    return this.roleUserCache !== true;
  }

  constructor(private http: HttpClient, private tokenService: TokenService, private userInfoService: UserInfoService) {
    this.baseUrlRegister = BASE_API_URL + '/auth/register';
    this.baseUrlLogin = BASE_API_URL + '/auth/login';
    this.baseUrlLogout = BASE_API_URL + '/auth/logout';
    this.baseUrlRefresh = BASE_API_URL + '/auth/refresh';
    this.baseUrlForget = BASE_API_URL + '/auth/forget';

    tokenService.TokenExpired.subscribe(() => {
      this.refresh();
    });
  }

  public register(registerFormModel: RegisterFormModel) {
    this.http.post(
      this.baseUrlRegister,
      JSON.stringify({
        userName: registerFormModel.userName,
        email: registerFormModel.email,
        password: registerFormModel.password,
        confirmPassword: registerFormModel.confirmPassword
      }),
      { headers: this.headers }
    ).subscribe(response => {
      }, err => { Logger.error('Register failed'); Logger.error(err); });
  }

  public login(email: string, password: string) {
    this.http.post(
      this.baseUrlLogin,
      JSON.stringify({ password: password, email: email }),
      { headers: this.headers }
    ).subscribe(response => {
        const token: JwtToken = new JwtToken(
          response['accessToken'],
          response['refreshToken'],
          response['expireOn']
        );
        this.tokenService.writeToken(token);
        this.fillRoles();
        this.AuthChanged.emit('Logged in');
      }, err => { Logger.error('Login failed'); Logger.error(err); });
  }

  public logout() {
    const jwtToken: JwtToken = this.tokenService.readJwtToken();
    this.http.post(
      this.baseUrlLogout,
      JSON.stringify({
        accessToken: jwtToken.accessToken,
        refreshToken: jwtToken.refreshToken,
        expireOn: jwtToken.expireOn
      }),
      { headers: this.headers }
    ).subscribe(() => {
      this.tokenService.deleteToken();
      this.clearRoles();
      this.AuthChanged.emit('Logged out');
    }, err => { Logger.error('Logout failed'); Logger.error(err); });
  }

  public refresh() {
    const jwtToken: JwtToken = this.tokenService.readJwtToken();
    this.http.post(
      this.baseUrlRefresh,
      JSON.stringify({
        accessToken: jwtToken.accessToken,
        refreshToken: jwtToken.refreshToken,
        expireOn: jwtToken.expireOn
      }),
      { headers: this.headers }
    ).subscribe(response => {
      const token: JwtToken = {
        accessToken: response['accessToken'],
        refreshToken: response['refreshToken'],
        expireOn: response['expireOn']
      };
      this.tokenService.writeToken(token);
    });
  }

  public forget(email: string) {
    this.http.post(
      this.baseUrlForget,
      JSON.stringify({ email: email }),
      { headers: this.headers }
    ).subscribe(response => {
      }, err => { Logger.error('Forget failed'); Logger.error(err); });
  }

  private fillRoles() {
    if (this.roleUserCache === null) {

      const tokenRoles = this.userInfoService.roles;

      if (tokenRoles != null) {
        this.roleAdminCache = this.roleUserCache = false;

        for (const role of tokenRoles) {
          if (role === 'User') {
            this.roleUserCache = true;
          } else if (role === 'Admin') {
            this.roleAdminCache = true;
          }
        }
      }
    }
  }

  private clearRoles() {
    this.roleAdminCache = this.roleUserCache = null;
  }
}
