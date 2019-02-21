import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Response } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Observable';
import * as jwt_decode from "jwt-decode";



@Injectable()
export class AuthService {

  private BaseUrlLogin = "https://localhost:44340/api/auth/login";

  constructor(private http: HttpClient) {
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
    }).map((response: Response) => response)
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

  checkAuth() {
    if (this.getEncodeToken() !== false) {
      let tokenInfo = this.getEncodeToken();
      return tokenInfo.sub;
    }
    return false;
  }

}
