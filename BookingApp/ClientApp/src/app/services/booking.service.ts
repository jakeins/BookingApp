import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Response } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import { BASE_API_URL } from '../globals';

import { TreeEntry } from '../models/tree-entry';
import { Booking } from '../models/booking';
import { Logger } from './logger.service';
import { DatePipe } from '@angular/common';

@Injectable()
export class BookingService {
  path: string;
  headers: HttpHeaders = new HttpHeaders({
    "Content-Type": "application/json",
    "Accept": "application/json"
  });

  constructor(private http: HttpClient) {
    this.path = BASE_API_URL + '/booking';
  }

  getBookings(startTime?: Date, endTime?: Date): Observable<Booking[]> {
    let datePipe = new DatePipe("en-Us");
    let query: String;
    query = "";
    if (!(startTime == undefined || startTime == null)) query = "?startTime=" + datePipe.transform(startTime, 'short');
    if (!(endTime == undefined || endTime == null)) {
      if (query == null) query = "?";
      else query += "&"
      query += "endTime=" + datePipe.transform(endTime, 'short');
    }
    return this.http.get(this.path + query,
      {
        headers: this.headers
      }).map((response: Response) => response)
      .catch((error: any) =>
        Observable.throw(error.error || 'Server error')); 
  }

  getBookingOfResource(resourceId: number, startTime?: Date, endTime?: Date): Observable<Booking[]> {
    let datePipe = new DatePipe("en-Us");
    let query: String;
    query = "";
    if (!(startTime == undefined || startTime == null)) query = "?startTime=" + datePipe.transform(startTime, 'short');
    if (!(endTime == undefined || endTime == null)) {
      if (query == null) query = "?";
      else query += "&"
      query += "endTime=" + datePipe.transform(endTime, 'short');
    }
    return this.http.get(BASE_API_URL + "/resources/" + resourceId + "/bookings" + query, 
      {
        headers: this.headers
      }).map((response: Response) => response)
      .catch((error: any) =>
        Observable.throw(error.error || 'Server error'));
  }

  getBooking(id: number): Observable<Booking> {
    return this.http.get<Booking>(this.path + '/' + id);
  }


  createBooking(booking: Booking): Observable<any> {
    let datePipe = new DatePipe("en-Us");
    return this.http.post(this.path,
      {
        startTime: datePipe.transform(booking.startTime, 'short'),
        endTime: datePipe.transform(booking.endTime, 'short'),
        note: booking.note,
        ResourceId: booking.resourceId,
        CreatedUserId: booking.createdUserId
      });
  }

  updateBooking(booking: Booking): Observable<any> {
    let datePipe = new DatePipe("en-Us");
    return this.http.put(this.path + '/' + booking.id, {
      startTime: datePipe.transform(booking.startTime, 'short'),
      endTime: datePipe.transform(booking.endTime, 'short'),
      note: booking.note,
    });
  }

  deleteBooking(id: number): Observable<any> {
    return this.http.delete(this.path + '/' + id);
  }

  terminateBooking(id: number): Observable<any> {
    return this.http.put(this.path + "/terminate/" + id, {});
  }
}
