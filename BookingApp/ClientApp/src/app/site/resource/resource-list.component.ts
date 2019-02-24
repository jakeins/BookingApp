import { Component, OnInit } from '@angular/core';

import { TreeEntry } from '../../models/tree-entry';
import { ResourceService } from '../../services/resource.service';


@Component({
  selector: 'app-resource-list',
  templateUrl: './resource-list.component.html'
})
export class ResourceListComponent implements OnInit {

  resources: TreeEntry;

  constructor(private folderService: ResourceService) { }

  ngOnInit() {
    this.folderService.getResources().subscribe((response: TreeEntry) => {
      this.resources = response;
    });
  }

}
