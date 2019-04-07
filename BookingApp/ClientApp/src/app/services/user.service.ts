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
import { UserNewPassword } from '../models/user-new-password';
import { AdminRegister } from '../models/admin-register';
import { DatePipe } from '@angular/common';


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

  createAdmin(user: AdminRegister): Observable<any> {
    return this.http.post(this.path + '/create-admin', user, { headers: this.headers });
  }

  updateUser(userId: string, user: UserUpdate): Observable<any> {
    return this.http.put(this.path + '/' + userId, user, { headers: this.headers });
  }

  deleteUser(userId: string): Observable<any> {
    return this.http.delete(this.path + '/' + userId);
  }

  getUserById(userId: string): Observable<User> {
    return this.http.get<User>(this.path + '/' + userId);
  }

  getUserByUserName(userName: string): Observable<User> {
    return this.http.get<User>(this.path + '/user-name/' + userName);
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

  getBookings(userId: string, startTime?: Date, endTime?: Date): Observable<any> {
    let datePipe = new DatePipe("en-Us");
    let query: String;
    query = "";
    if (!(startTime == undefined || startTime == null)) query = "?startTime=" + datePipe.transform(startTime, 'short');
    if (!(endTime == undefined || endTime == null)) {
      if (query == null) query = "?";
      else query += "&"
      query += "endTime=" + datePipe.transform(endTime, 'short');
    }
    return this.http.get(this.path + '/' + userId + '/bookings' + query, { headers: this.headers });
  }

  blockUser(userId: string, blocking: boolean): Observable<Object> {
    return this.http.put(this.path + '/' + userId + '/blocking', blocking, { headers: this.headers});
  }

  approvalUser(userId: string, approval: boolean): Observable<Object> {
    return this.http.put(this.path + '/' + userId + '/approval', approval, { headers: this.headers });
  }

  ressetPassword(userId: string, code: string, userPass: UserNewPassword): Observable<Object> {
    return this.http.put(this.path + '/' + userId + '/reset-password/' + code, userPass, { headers: this.headers });
  }

  changePassword(userId: string, userPass: UserNewPassword): Observable<Object> {
    return this.http.put(this.path + '/' + userId + '/change-password', userPass, { headers: this.headers });
  }
  
  getUserName(): any {
    return this.userInfoService.username;
  }
}
