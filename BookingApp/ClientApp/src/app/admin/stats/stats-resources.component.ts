import { Component, OnInit, ViewChild } from '@angular/core';
import { StatsService } from '../../services/stats.service';
import { ResourceStatsBrief } from '../../models/stats-resource-brief';
import { Logger } from '../../services/logger.service';

@Component({
  selector: 'app-stats-resources',
  templateUrl: './stats-resources.component.html',
  styleUrls: ['./stats-resources.component.css']
})
export class StatsResourcesComponent implements OnInit {

  resourcesRating: ResourceStatsBrief[];

  constructor(private statsService: StatsService ) {     
  }

  ngOnInit() {
    this.statsService.getResourcesRating().subscribe((res: ResourceStatsBrief[]) => {
      this.resourcesRating = res;
    })
  }  
}
