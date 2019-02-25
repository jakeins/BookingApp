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
      return response;
      })
      .catch((error: any) =>
        Observable.throw(error.error || 'Server error'));
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

  getUserName() {
    if (this.getEncodeToken() !== false) {
      let tokenInfo = this.getEncodeToken();
      return tokenInfo.sub;
    }
    return false;
  }


  isAdmin: boolean = false;
  isUser: boolean = false;

  authAsAdmin() {
    let login = "superadmin@admin.cow";
    let password = "SuperAdmin";
    this.login(login, password)
      .subscribe(res => {
        if (res != null) {
          this.isAdmin = true;
          this.isUser = true;
        }
      });
  }

  authAsUser() {
    let login = "lion@user.cow";
    let password = "Lion";
    this.login(login, password)
      .subscribe(res => {
        if (res != null) {
          this.isUser = true;
          this.isAdmin = false;
        }
      });
  }

  logout() {
    this.removeToken();
    this.isAdmin = false;
    this.isUser = false;
  }


  resetAuthFlags() {
    let userName = this.getUserName();

      if (userName == "Lion") {
        this.isAdmin = false;
        this.isUser = true;
      }
      else if (userName == "SuperAdmin") {
        this.isAdmin = true;
        this.isUser = true;
      }
      else {
        this.isAdmin = false;
        this.isUser = false;
      }
  }



}
