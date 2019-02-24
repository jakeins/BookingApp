import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';

import { BASE_API_URL } from '../globals';

import { TreeEntry } from '../models/tree-entry';
import { Resource } from '../models/resource';

@Injectable()
export class ResourceService {
  path : string; 

constructor(private http: HttpClient) {
  this.path = BASE_API_URL + '/resources';
}

  getResources(): Observable<TreeEntry> {
    return this.http.get<TreeEntry>(this.path);
  }

  getResource(id: number): Observable<Resource> {
    return this.http.get<Resource>(this.path + '/' + id);
  }

}
