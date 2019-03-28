import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Observable } from 'rxjs';
import { tap, catchError} from 'rxjs/operators';
import { rule } from '../models/rule';
import { BASE_API_URL } from '../globals';
import { Logger } from './logger.service';

@Injectable()
export class RuleService {
  showAdditionalInfo: boolean = false;
  Rule: rule;
  Rules: rule[];
  url: string = BASE_API_URL + '/rules';
  constructor(
    private http: HttpClient,
    ) { }

  refreshList(){
    this.http.get<rule[]>(this.url).toPromise().then((res: rule[]) => {
      console.log("list was refreshed");
      this.Rules = res; 
    },
    catchError(this.handleError<rule[]>('refresh list', []))
    )
  }

  getRule(id: number): Observable<rule> {
    return this.http.get<rule>(this.url + '/' + id).pipe(
      tap(_ => Logger.log(`${id} rule was loaded`)),
      catchError(this.handleError<rule>('get rule'))
    );
  }

  getRules(): Observable<rule[]> {
    return this.http.get<rule[]>(this.url).pipe(
      tap(_ => Logger.log("rules were loaded")),
      catchError(this.handleError<rule[]>('update rule',[]))
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

  //TODO: setup error handler
  private handleError<T> (operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      Logger.error(error); 
      console.log(`${operation} failed: ${error.message}`); //for feature for development
      return Observable.throw(error);
    };
  }

}
