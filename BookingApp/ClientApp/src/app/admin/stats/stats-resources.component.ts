import { Component, OnInit } from '@angular/core';
import { StatsService } from '../../services/stats.service';
import { ResourceStatsBrief } from '../../models/stats-resource-brief';

@Component({
  selector: 'app-stats-resources',
  templateUrl: './stats-resources.component.html',
  styleUrls: ['./stats-resources.component.css']
})
export class StatsResourcesComponent implements OnInit {

  resourcesRating: ResourceStatsBrief[];
  showing: boolean[] = [];
  constructor(private statsService: StatsService ) {     
  }

  ngOnInit() {
    this.statsService.getResourcesRating().subscribe((res: ResourceStatsBrief[]) => {
      this.resourcesRating = res;
      for (let i = 0; i < res.length; i++) {
        this.showing[i] = false;
      }      
      console.log('resources init');
      //for (let resource of this.resourcesRating) {
      //  document.getElementById(`btn${resource.id}`).onclick = () => { this.showResource(resource.id); };
      //}
    })
  } 

  showResource(id: number) {
    // change button text to hide details
    this.showing[id] = !this.showing[id];
  }
}
