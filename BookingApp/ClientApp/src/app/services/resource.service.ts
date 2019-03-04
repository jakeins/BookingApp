import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';

import { BASE_API_URL } from '../globals';

import { TreeEntry } from '../models/tree-entry';
import { Resource } from '../models/resource';
import { Logger } from './logger.service';

@Injectable()
export class ResourceService {
  path : string; 

constructor(private http: HttpClient) {
  this.path = BASE_API_URL + '/resources';
}

  getResources(): Observable<Resource[]> {
    return this.http.get<Resource[]>(this.path);
  }

  resetOccupancies(entries: TreeEntry[]) {
    this.http.get<number[]>(this.path + '/occupancy').subscribe((occupancies: number[]) => {

      for (let entry of entries) {
        let key = entry.id;

        //Logger.log(`${key} (${entry.title})`);

        if (occupancies[key] !== undefined) {

          let occupancy = occupancies[key] * 100;
          let title: string;

          if (occupancy <= 0)
            title = "Free";
          else if (occupancy < 30)
            title = "Almost free";
          else if (occupancy < 50)
            title = "Moderately occupied";
          else if (occupancy < 100)
            title = "Heavily occupied";
          else
            title = "Fully occupied";

          //Logger.log(`: ${occupancy} - ${title}`);

          entry.occupancy = occupancy;
          entry.occupancyTitle = title;
        }

      }
    });
  }

  getResource(id: number): Observable<Resource> {
    return this.http.get<Resource>(this.path + '/' + id);
  }


  createResource(resource: Resource): Observable<any> {
    return this.http.post(this.path, resource);
  }

  updateResource(resource: Resource): Observable<any> {
    return this.http.put(this.path + '/' + resource.id, resource);
  }

  deleteResource(id: number): Observable<any> {
    return this.http.delete(this.path + '/' + id);
  }

}
