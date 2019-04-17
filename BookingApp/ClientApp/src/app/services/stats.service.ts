import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs/Observable';
import { BASE_API_URL } from '../globals';
import { BookingStats } from '../models/stats-booking';
import { ResourceStats } from '../models/stats-resource';
import { ResourceStatsBrief } from '../models/stats-resource-brief';
import { UsersStats } from '../models/stats-users';

@Injectable()
export class StatsService {

  url: string;

  constructor(private http: HttpClient) {
    this.url = BASE_API_URL + '/stats';
  }

  // temp
  getBookingStats(type: string, start: Date, end: Date, interval: string): Observable<BookingStats> {
    let startString = start.toDateString();
    let startDay = startString.substr(8, 2);
    let startMonth = startString.substr(4, 3);
    let startYear = startString.substr(11, 4);
    let endString = end.toDateString();
    let endDay = endString.substr(8, 2);
    let endMonth = endString.substr(4, 3);
    let endYear = endString.substr(11, 4);
    return this.http.get<BookingStats>(this.url + `/bookings-${type}?startTime=${startDay}%20${startMonth}%20${startYear}&endTime=${endDay}%20${endMonth}%20${endYear}&interval=${interval}`);
  }

  getResourceStats(id: number): Observable<ResourceStats> {
    return this.http.get<ResourceStats>(this.url + `/resources/${id}`);
  }

  getResourcesRating(): Observable<ResourceStatsBrief[]> {
    return this.http.get<ResourceStatsBrief[]>(this.url + `/resources-rating`);
  }

  getUsersStats(): Observable<UsersStats> {
    return this.http.get<UsersStats>(this.url + `/users`);
  }

}
