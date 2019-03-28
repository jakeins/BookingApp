import { Injectable, Output, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Logger } from './logger.service';
import { JwtToken } from '../models/jwt-token';
import { BASE_API_URL } from '../globals';
import { TokenService } from './token.service';
import { UserInfoService } from './user-info.service';
import { RegisterFormModel } from '../models/register-form.model';
import { Observable } from 'rxjs/Observable';
import { map, finalize } from 'rxjs/operators';

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
      this.refresh().subscribe(data => { }, err => console.log(err));
    });
  }

  public register(registerFormModel: RegisterFormModel): Observable<any> {
    return this.http.post(
      this.baseUrlRegister,
      JSON.stringify({
        userName: registerFormModel.userName,
        email: registerFormModel.email,
        password: registerFormModel.password,
        confirmPassword: registerFormModel.confirmPassword
      }),
      { headers: this.headers }
    );
  }

  public login(email: string, password: string) {
    return this.http.post(
      this.baseUrlLogin,
      JSON.stringify({ password: password, email: email }),
      { headers: this.headers }
    ).pipe(map(data => {
      const token: JwtToken = new JwtToken(
        data['accessToken'],
        data['refreshToken'],
        data['expireOn']
      );
      this.tokenService.writeToken(token);
      this.fillRoles();
      this.AuthChanged.emit('Logged in');
    }));
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
    ).subscribe(data => {}, err => console.log(err));
    this.tokenService.deleteToken();
    this.clearRoles();
    this.AuthChanged.emit('Logged out');
  }

  public refresh() {
    const jwtToken: JwtToken = this.tokenService.readJwtToken();
    return this.http.post(
      this.baseUrlRefresh,
      JSON.stringify({
        accessToken: jwtToken.accessToken,
        refreshToken: jwtToken.refreshToken,
        expireOn: jwtToken.expireOn
      }),
      { headers: this.headers }
    ).pipe(map(data => {
      const token: JwtToken = {
        accessToken: data['accessToken'],
        refreshToken: data['refreshToken'],
        expireOn: data['expireOn']
      };
      this.tokenService.writeToken(token);
    }));
  }

  public forget(email: string): Observable<any> {
    return this.http.post(
      this.baseUrlForget,
      JSON.stringify({ email: email }),
      { headers: this.headers }
    );
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
