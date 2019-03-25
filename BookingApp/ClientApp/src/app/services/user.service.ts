import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Response } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import * as jwt_decode from "jwt-decode";
import { Observable } from 'rxjs/Observable';
import { User } from '../models/user';
import { BASE_API_URL } from '../globals';
import { UserRegister } from '../models/user-register';
import { UserPage } from '../models/user-page';
import { UserUpdate } from '../models/user-update';
import { UserInfoService } from './user-info.service';


@Injectable()
export class UserService {
  private defaultPath: string;
  private path: string;
  private baseApiUrl: string;
  private userRegister: UserRegister;
  //private tokenservice: AccessTokenService;

  headers: HttpHeaders = new HttpHeaders({
    "Content-Type": "application/json",
    "Accept": "application/json"
  });
  constructor(private http: HttpClient, private userInfoService: UserInfoService) {
    this.defaultPath = BASE_API_URL + '/users';
    this.path = BASE_API_URL + '/user';
    this.baseApiUrl = BASE_API_URL;
  }

  createUser(user: UserRegister): Observable<any> {
    return this.http.post(this.path, user,  { headers: this.headers });
  }

  createAdmin(user: UserRegister): Observable<any> {
    return this.http.post(this.path + '/crate-admin', user, { headers: this.headers });
  }

  updateUser(user: UserUpdate, userId: string): Observable<any> {
    return this.http.put(this.path + '/' + userId, user, { headers: this.headers });
  }

  deleteUser(userId: string): Observable<any> {
    return this.http.delete(this.path + '/' + userId);
  }

  getUserById(userId: string): Observable<User> {
    return this.http.get<User>(this.path + '/' + userId);
  }

  getUserRoleById(userId: string): Observable<string[]> {
    return this.http.get<string[]>(this.path + '/' + userId + '/roles');
  }

  getUserByEmail(userEmail: string): Observable<User> {
    return this.http.get<User>(this.path + '/email/' + userEmail);
  }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.defaultPath);
  }

  getUsersById(usersId: string[]): Observable<User[]> {
    return this.http.post<User[]>(this.path + '/users-by-id', usersId, { headers: this.headers });
  }

  getUsersPage(page: number, pageSize: number): Observable<UserPage> {
    return this.http.get<UserPage>(this.path + '/page' + '?' + 'PageNumber=' + page + '&' + 'PageSize=' + pageSize);
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
    return this.userInfoService.username;
  }
}
