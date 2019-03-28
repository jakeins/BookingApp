import { Component, OnInit } from '@angular/core';
import { StatsService } from '../../services/stats.service';
import { BookingStats } from '../../models/stats-booking';
import { Logger } from '../../services/logger.service';
import { Data } from '@angular/router';
import { logging } from 'protractor';


@Component({
  selector: 'app-stats-booking',
  templateUrl: './stats-bookings.component.html',
  styleUrls: ['./stats-bookings.component.css']
})
export class StatsBookingComponent implements OnInit {

  bookingStats: BookingStats;
  
  defaultFromDate: Date;
  defaultToDate: Date;

  fromDateString: string;
  toDateString: string;

  actionsTypeSelect: string = 'Creations';

  constructor(private statsService: StatsService ) {     
  }

  ngOnInit() {
    let currentDate: Date;
    currentDate = new Date();
    Logger.log('currentDate: '+currentDate);
    this.defaultToDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate());
    this.defaultFromDate = new Date(this.defaultToDate.getTime() - (1000 * 60 * 60 * 24 * 7));

    this.fromDateString = this.dateToString(this.defaultFromDate);
    Logger.log('fromDate: ' + this.fromDateString);
    //(document.getElementById('fromDate') as HTMLInputElement).value = this.fromDateString;
    Logger.log('html fromDate: ' + (document.getElementById('fromDate') as HTMLInputElement).value);
    this.toDateString = this.dateToString(this.defaultToDate);
    Logger.log('toDate: ' + this.toDateString);
    //(document.getElementById('toDate') as HTMLInputElement).value = this.toDateString; 
  }

  loadStats() {
    this.statsService.getBookingStats(this.actionsTypeSelect).subscribe((res: BookingStats) => {
      this.bookingStats = res;
    });
  }

  dateToString(date: Date) {
    let year = date.getFullYear().toString();
    let month = date.getMonth().toString().length < 2 ? '0' + (date.getMonth()+1).toString() : (date.getMonth()+1).toString();
    let day = date.getDate().toString().length < 2 ? '0' + date.getDate().toString() : date.getDate().toString();
    return year + '-' + month + '-' + day;
  }
}
