import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';

import { BASE_API_URL } from '../globals';

import { TreeEntry } from '../models/tree-entry';
import { Resource } from '../models/resource';
import { Logger } from './logger.service';

@Injectable()
export class ResourceService {
  path: string;
  public ResourceCache = Object.create(null);

constructor(private http: HttpClient) {
  this.path = BASE_API_URL + '/resources';
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




  createResource(resource: Resource): Observable<any> {
    return this.http.post(this.path, resource);
  }

  updateResource(resource: Resource): Observable<any> {
    return this.http.put(this.path + '/' + resource.id, resource);
  }

  deleteResource(id: number): Observable<any> {
    return this.http.delete(this.path + '/' + id);
  }



  getResource(resourceID: number): Observable<Resource> {

    if (resourceID == undefined || resourceID < 1) {
      Logger.error(`Resource ID [${resourceID}] is illegal.`);
      return null;
    }

    const obs = this.http.get<Resource>(this.path + '/' + resourceID);
    obs.subscribe(result => this.updateResourceCache(result));
    return obs;
  }

  getResources(): Observable<Resource[]> {

    const obs = this.http.get<Resource[]>(this.path);
    obs.subscribe(result => {
      for (let resource of result) {
        this.updateResourceCache(resource);
      }
    });
    return obs;
  }






  updateResourceCache(resource: Resource): void {
    this.ResourceCache[resource.id] = resource;
    Logger.log(`Resource cache for [${resource.title}] updated.`);
  }

  getResourceCache(resourceId: string): Resource {
    return this.ResourceCache[resourceId];
  }

  getResourcesCache(resourceIds: string[]): Resource[] {
    let result: Resource[] = [];

    for (let resourceID of resourceIds) {
      result.push(this.ResourceCache[resourceID]);
    }

    return result;
  }


}
