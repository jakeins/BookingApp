import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Response } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Observable';
import * as jwt_decode from "jwt-decode";
import { DOCUMENT } from '@angular/common';
import { Logger } from './logger.service';

@Injectable()
export class AuthService {

  private BaseUrlLogin: string;

  constructor(private http: HttpClient, @Inject(DOCUMENT) private document: any) {
    this.BaseUrlLogin = document.location.protocol + '/api/auth/login';
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
  }






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

  resetAuthFlags() {
    this.resetTokenRoles();
  }

  setToken(accessToken: string): void {
    localStorage.setItem('accessToken', accessToken);
  }

  removeToken(): void {
    localStorage.removeItem('accessToken');
  }

  getEncodeToken() {
    try {
      let token = localStorage.getItem('accessToken');
      return jwt_decode(token);
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
