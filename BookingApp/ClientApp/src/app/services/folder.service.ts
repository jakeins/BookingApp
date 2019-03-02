import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Response } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Observable';
import { Folder } from '../models/folder';
import { BASE_API_URL } from '../globals';


@Injectable()
export class FolderService {
  private BaseUrlFolder: string;

  constructor(private http: HttpClient) {
    this.BaseUrlFolder = BASE_API_URL + '/folder';
  }

  public getList(): Observable<Folder> {
    var headers = new HttpHeaders({
      "Content-Type": "application/json",
      "Accept": "application/json"
    });
    return this.http.get(this.BaseUrlFolder, {
      headers: headers
    }).map((response: Response) => response)
      .catch((error: any) =>
        Observable.throw(error.error || 'Server error'));
  }

  public newRoot() {
    return new Folder(0, "root", true, null);
  }

}
