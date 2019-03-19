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
  headers: HttpHeaders = new HttpHeaders({
    "Content-Type": "application/json",
    "Accept": "application/json"
  });

  constructor(private http: HttpClient) {
    this.BaseUrlFolder = BASE_API_URL + '/folder';
  }

  public getList(): Observable<Folder[]> {
    return this.http.get(this.BaseUrlFolder, {
      headers: this.headers
    }).map((response: Response) => response)
      .catch((error: any) =>
        Observable.throw(error.error || 'Server error'));
  }

  getFolder(id: number): Observable<Folder> {
    return this.http.get<Folder>(this.BaseUrlFolder + '/' + id, {
      headers: this.headers
    }).map((response: Folder) => { return response; })
      .catch((error: any) =>
        Observable.throw(error.error || 'Server error'));
  }

  createFolder(folder: Folder): Observable<any> {
    return this.http.post(this.BaseUrlFolder, folder, {
      headers: this.headers
    })
  }

  updateFolder(folder: Folder): Observable<any> {
    return this.http.put(this.BaseUrlFolder + '/' + folder.id, folder, {
      headers: this.headers
    })
  }

  deleteFolder(id: number): Observable<any> {
    return this.http.delete(this.BaseUrlFolder + '/' + id, {
      headers: this.headers
    })
  }

  public newRoot() {
    return new Folder("root", null, null, false, 0);
  }

  public getMockRules() {
    return [
      { "id": 1, "title": "Default"},
      { "id": 2, "title": "Rule 2" },
      { "id": 3, "title": "Rule 3" },
      { "id": 4, "title": "Rule 4" },
      { "id": 5, "title": "Rule 5" },
      { "id": 6, "title": "Rule 6" },
    ];
  }

}
