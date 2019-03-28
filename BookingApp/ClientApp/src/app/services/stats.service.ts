import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs/Observable';
import { BASE_API_URL } from '../globals';
import { BookingStats } from '../models/stats-booking';

@Injectable()
export class StatsService {

  url: string;

  constructor(private http: HttpClient) {
    this.url = BASE_API_URL + '/statistics';
  }

  // temp
  getBookingStats(): Observable<BookingStats> {
    return this.http.get<BookingStats>(this.url + `/bookings-creations?startTime=01%20Mar%202019&endTime=05%20Mar%202019&interval=day`);
  }

}
