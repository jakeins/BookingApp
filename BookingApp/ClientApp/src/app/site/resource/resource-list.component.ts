import { Component, OnInit } from '@angular/core';

import { TreeEntry } from '../../models/tree-entry';
import { ResourceService } from '../../services/resource.service';
import { Resource } from '../../models/resource';


@Component({
  selector: 'app-resource-list',
  templateUrl: './resource-list.component.html',
  styleUrls: ['./resource-list.component.css']
})
export class ResourceListComponent implements OnInit {

  resourceEntries: TreeEntry[] = [];

  constructor(private resourceService: ResourceService) { }

  ngOnInit() {
    this.resourceService.getResources().subscribe((result: Resource[]) => {

      for (let key in result) {
        this.resourceEntries[key] = new TreeEntry(result[key]);
      }

      this.resourceService.resetOccupancies(this.resourceEntries);
    });
    
  }
}
