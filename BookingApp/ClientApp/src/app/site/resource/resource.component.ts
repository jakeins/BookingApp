import { Component, OnInit } from '@angular/core';

import { TreeEntry } from '../../models/tree-entry';
import { ResourceService } from '../../services/resource.service';
import { Resource } from '../../models/resource';
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-resource',
  templateUrl: './resource.component.html',
  styleUrls: ['./resource.component.css']
})
export class ResourceComponent implements OnInit {

  resource: Resource;
  id: number;

  constructor(private folderService: ResourceService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.id = +params['id'];
    });

    this.folderService.getResource(this.id).subscribe((response: Resource) => {
      this.resource = response;
    });
  }

}
