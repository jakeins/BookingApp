import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs';
import { tap, catchError} from 'rxjs/operators';
import { rule } from '../models/rule';
import { BASE_API_URL } from '../globals';
import { Logger } from './logger.service';


@Injectable()
export class RuleService {

  url: string = BASE_API_URL + '/rules';
  constructor(
    private http: HttpClient
    ) { }

  getRule(id: number): Observable<rule> {
    return this.http.get<rule>(this.url + '/' + id).pipe(
      tap(_ => Logger.log(`${id} rule was loaded`)),
      catchError(this.handleError<rule>('get rule'))
    );
  }


  getRules(): Observable<rule[]> {
    return this.http.get<rule[]>(this.url).pipe(
      tap(_ => Logger.log("rules were loaded")),
      catchError(this.handleError<rule[]>('get rules',[]))
    );
  }

  addRule(rule: rule): Observable<rule> {
    return this.http.post<rule>(this.url, rule).pipe(
      tap(_ => Logger.log("rule was added")),
      catchError(this.handleError<rule>('add rule'))
    );
  }

  updateRule(rule: rule): Observable<rule> {
     return this.http.put<rule>(this.url+ `/${rule.id}`, rule).pipe(
       tap(_ => Logger.log(`${rule.id} rule was updated`)),
       catchError(this.handleError<rule>('update rule'))
     );
  }


  deleteRule(id:number): Observable<rule> {
     return this.http.delete<rule>(this.url + `/${id}`).pipe(
       tap(_ => Logger.warn(`${id} rule was deleted`)),
       catchError(this.handleError<rule>('delete rule'))
     );
  }
  
  private handleError<T> (operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      Logger.error(error); 
      console.log(`${operation} failed: ${error.message}`); //feature for development 
      return Observable.throw(error);
    };
  }

}
