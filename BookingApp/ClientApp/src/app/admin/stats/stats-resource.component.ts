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

  stringToTime(raw: string) {
    let hours: number = (Number)(raw.substr(0, 2));
    let minutes: number = (Number)(raw.substr(3, 2));
    let seconds: number = (Number)(raw.substr(6, 2));

    let result: string = "";
    if (hours > 0)
      result += hours.toString() + " hours ";
    if (minutes > 0)
      result += minutes.toString() + " minutes ";
    if (seconds > 0)
      result += seconds.toString() + " seconds";

    return result;
  }
}
