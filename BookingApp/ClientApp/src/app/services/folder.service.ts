import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Response } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Observable';
import { Folders } from '../models/folders';
import { DOCUMENT } from '@angular/common';


@Injectable()
export class FolderService {
  private BaseUrlFolder: string;

  constructor(private http: HttpClient, @Inject(DOCUMENT) private document: any) {
    this.BaseUrlFolder = document.location.protocol + '/api/folder';
  }

  getList(): Observable<Folders> {
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

}
