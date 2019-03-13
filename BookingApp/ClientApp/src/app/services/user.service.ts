import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Response } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Observable';
import { User } from '../models/user';
import { BASE_API_URL } from '../globals';
import { Resource } from '../models/resource';
import { Blocking} from '../models/UserBlockingDto';
import { UserPage } from '../models/user-page';


@Injectable()
export class UserService {
  private defaultPath: string;
  private path: string;
  private baseApiUrl: string;
  public blockingModel: Blocking;
  headers: HttpHeaders = new HttpHeaders({
    "Content-Type": "application/json",
    "Accept": "application/json"
  });
  constructor(private http: HttpClient) {
    this.defaultPath = BASE_API_URL + '/users';
    this.path = BASE_API_URL + '/user';
    this.baseApiUrl = BASE_API_URL;
  }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.defaultPath);
  }

  getUsersPage(page: number, pageSize: number): Observable<UserPage> {
    return this.http.get<UserPage>(this.baseApiUrl + '/' + 'users-page' + '?' + 'PageNumber=' + page + '&' + 'PageSize=' + pageSize);
  }

  blockUser(userId: string, blocking: Blocking): Observable<Object> {
   // this.blockingModel.Is
    console.log(blocking);
    return this.http.put(this.path + '/' + userId + '/blocking', blocking, { headers: this.headers});
  }
}
