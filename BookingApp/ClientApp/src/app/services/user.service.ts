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


@Injectable()
export class UserService {
  private defaultPath: string;
  private path: string;
  public blockingModel: Blocking;
  
  constructor(private http: HttpClient) {
    this.defaultPath = BASE_API_URL + '/users';
    this.path = BASE_API_URL + '/user';
  }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.defaultPath);
  }

  blockUser(userId: string, blocking: Blocking): Observable<any> {
   // this.blockingModel.Is
    console.log(blocking);
    return this.http.put(this.path + '/' + userId + '/blocking', blocking);
  }
}
