import { Injectable, Inject, Output, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Response } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Observable';
import * as jwt_decode from "jwt-decode";
import { Logger } from './logger.service';
import { BASE_API_URL } from '../globals';

@Injectable()
export class AuthService {

  private BaseUrlLogin: string;

  constructor(private http: HttpClient) {
    this.BaseUrlLogin = BASE_API_URL + '/auth/login';
  }

  login(login, password) {

    var headers = new HttpHeaders({
      "Content-Type": "application/json",
      "Accept": "application/json"
    });
    let postData = {
      password: password,
      email: login
    }

    return this.http.post(this.BaseUrlLogin, JSON.stringify(postData), {
      headers: headers
    }).map((response: Response) => {

      //Logger.log(response);

      this.setToken(response.toString());
      this.resetTokenRoles();
      this.AuthChanged.emit('Logged in');

      return response;
      })
      .catch((error: any) =>
        Observable.throw(error.error || 'Server error'));
  }

  authAsAdmin() {
    this.login("superadmin@admin.cow", "SuperAdmin").subscribe();
  }

  authAsUser() {
    this.login("lion@user.cow", "Lion").subscribe();
  }

  logout() {
    this.removeToken();
    this.resetTokenRoles();
    this.AuthChanged.emit('Logged out');
  }


  @Output() AuthChanged: EventEmitter<any> = new EventEmitter();




  _isAdmin: boolean = false;
  _isUser: boolean = false;

  get isAdmin(): boolean {
    return this._isAdmin;
  }

  get isUser(): boolean {
    return this._isUser;
  }

  get isAnonymous(): boolean {
    return !this.isUser;
  }

  setToken(accessToken: string): void {
    localStorage.setItem('accessToken', accessToken);
  }

  getToken(): string {
    return localStorage.getItem('accessToken');
  }

  removeToken(): void {
    localStorage.removeItem('accessToken');
  }

  getEncodeToken() {
    try {
      return jwt_decode(this.getToken());
    } catch (e) {
      return false;
    }
  }

  getTokenUserName() {
    if (this.getEncodeToken() !== false) {
      let tokenInfo = this.getEncodeToken();

      Logger.log(tokenInfo);

      return tokenInfo.sub;
    }
    return false;
  }

  resetTokenRoles() {
    this._isUser = this._isAdmin = false;

    if (this.getEncodeToken() !== false) {
      let roles = this.getEncodeToken()["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

      if (!(roles instanceof Array))
        roles = [roles];

      for (let x of roles) {
        if (x == "User")
          this._isUser = true;
        else if (x == "Admin")
          this._isAdmin = true;
      }
    }
  }


}
