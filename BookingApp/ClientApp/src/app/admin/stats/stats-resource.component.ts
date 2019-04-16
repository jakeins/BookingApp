import { Component, OnInit,OnChanges, Input } from '@angular/core';
import { StatsService } from '../../services/stats.service';
import { ResourceStats } from '../../models/stats-resource';
import { Logger } from '../../services/logger.service';

@Component({
  selector: 'app-stats-resource',
  templateUrl: './stats-resource.component.html',
  styleUrls: ['./stats-resource.component.css']
})
export class StatsResourceComponent implements OnInit {

  @Input() resourceId: number;
  @Input() shown: boolean;
  
  resourceStats: ResourceStats;  

  constructor(private statsService: StatsService) {
  }

  ngOnInit() {
    this.statsService.getResourceStats(this.resourceId).subscribe((res: ResourceStats) => {
      this.resourceStats = res;
    })
  }  
}
