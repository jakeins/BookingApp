import { Injectable, Output, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Logger } from './logger.service';
import { BASE_API_URL } from '../globals';
import { AccessTokenService } from './access-token.service';

@Injectable()
export class AuthService {

  private BaseUrlLogin: string;
  @Output() AuthChanged: EventEmitter<any> = new EventEmitter();
  
  public get isAdmin() : boolean {
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

  constructor(private http: HttpClient, private aTokenService : AccessTokenService) {
    this.BaseUrlLogin = BASE_API_URL + '/auth/login';

    aTokenService.TokenExpired.subscribe(() => {
      this.logout();
    })
  }

  public login(login, password) {

    this.http.post(
      this.BaseUrlLogin,
      JSON.stringify({ password: password, email: login }),
      { headers: new HttpHeaders({ "Content-Type": "application/json", "Accept": "application/json" }) }
      )
      .subscribe((response: Response) => {
        this.aTokenService.writeToken(response.toString());
        this.fillRoles();
        this.AuthChanged.emit('Logged in');
      }, error => Observable.throw(error.error || 'Server error'));
  }

  public logout() {
    this.aTokenService.deleteToken();
    this.clearRoles();
    this.AuthChanged.emit('Logged out');
  }

  private roleAdminCache?: boolean = null;
  private roleUserCache?: boolean = null;
  private fillRoles() {
    if (this.roleUserCache === null) {
      
      let tokenRoles = this.aTokenService.readRoles();

      if (tokenRoles != undefined) {
        this.roleAdminCache = this.roleUserCache = false;

        for (let role of tokenRoles) {
          if (role == "User")
            this.roleUserCache = true;
          else if (role == "Admin")
            this.roleAdminCache = true;
        }
      }
    }
  }
  private clearRoles() {
    this.roleAdminCache = this.roleUserCache = null;
  }
}
