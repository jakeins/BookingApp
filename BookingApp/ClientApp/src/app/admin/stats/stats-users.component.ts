import { Component, OnInit } from '@angular/core';
import { StatsService } from '../../services/stats.service';
import { UsersStats } from '../../models/stats-users';

@Component({
  selector: 'app-stats-users',
  templateUrl: './stats-users.component.html',
  styleUrls: ['./stats-users.component.css']
})
export class StatsUsersComponent implements OnInit {

  userStats: UsersStats;

  constructor(private statsService: StatsService ) {     
  }

  ngOnInit() {
    this.statsService.getUsersStats().subscribe((res: UsersStats) => {
      this.userStats = res;
    })
  }  
}
