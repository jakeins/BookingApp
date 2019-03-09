import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Response } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Observable';
import { User } from '../models/user';
import { BASE_API_URL } from '../globals';
import {Resource} from '../models/resource';


@Injectable()
export class UserService {
  private path: string;

  constructor(private http: HttpClient) {
    this.path = BASE_API_URL + '/users';
  }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.path);
  }
}
