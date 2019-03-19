import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Response } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import * as jwt_decode from "jwt-decode";
import { Observable } from 'rxjs/Observable';
import { AccessTokenService } from './access-token.service';
import { User } from '../models/user';
import { BASE_API_URL } from '../globals';
import { Resource } from '../models/resource';
import { UserPage } from '../models/user-page';


@Injectable()
export class UserService {
  private defaultPath: string;
  private path: string;
  private baseApiUrl: string;
  //private tokenservice: AccessTokenService;

  headers: HttpHeaders = new HttpHeaders({
    "Content-Type": "application/json",
    "Accept": "application/json"
  });
  constructor(private http: HttpClient, private tokenservice: AccessTokenService) {
    this.defaultPath = BASE_API_URL + '/users';
    this.path = BASE_API_URL + '/user';
    this.baseApiUrl = BASE_API_URL;
  }

  getUserById(userId: string): Observable<User> {
    return this.http.get<User>(this.path + '/' + userId);
  }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.defaultPath);
  }

  getUsersPage(page: number, pageSize: number): Observable<UserPage> {
    return this.http.get<UserPage>(this.baseApiUrl + '/' + 'users-page' + '?' + 'PageNumber=' + page + '&' + 'PageSize=' + pageSize);
  }

  blockUser(userId: string, blocking: boolean): Observable<Object> {
   // this.blockingModel.Is
    console.log(blocking);
    return this.http.put(this.path + '/' + userId + '/blocking', blocking, { headers: this.headers});
  }

  approvalUser(userId: string, approval: boolean): Observable<Object> {
    return this.http.put(this.path + '/' + userId + '/approval', approval, { headers: this.headers });
  }

  getUserName(): any {
    return this.tokenservice.readUsername();
  }
}
