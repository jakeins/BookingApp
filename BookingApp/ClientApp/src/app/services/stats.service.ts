import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs/Observable';
import { BASE_API_URL } from '../globals';
import { BookingStats } from '../models/stats-booking';
import { Logger } from './logger.service';

@Injectable()
export class StatsService {

  url: string;

  constructor(private http: HttpClient) {
    this.url = BASE_API_URL + '/stats';
  }

  // temp
  getBookingStats(type: string, start: Date, end: Date, interval: string): Observable<BookingStats> {
    let startString = start.toString();
    Logger.log(startString);
    let startDay = startString.substr(8, 2);
    let startMonth = startString.substr(4, 3);
    let startYear = startString.substr(11, 4);
    let endString = end.toString();
    let endDay = endString.substr(8, 2);
    let endMonth = endString.substr(4, 3);
    let endYear = endString.substr(11, 4);
    return this.http.get<BookingStats>(this.url + `/bookings-${type}?startTime=${startDay}%20${startMonth}%20${startYear}&endTime=${endDay}%20${endMonth}%20${endYear}&interval=${interval}`);
  }

}
