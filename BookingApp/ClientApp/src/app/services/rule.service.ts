import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs/Observable';
import { tap, catchError } from 'rxjs/operators';
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
    error => Logger.error(error.message));
  }

  getRule(id: number): Observable<rule> {
    return this.http.get<rule>(this.url + '/' + id);
  }

  getRules(): Observable<rule[]> {
    return this.http.get<rule[]>(this.url).pipe(
      tap(_ => Logger.log("rules were loaded"))
    );
  }

  addRule(rule: rule): Observable<rule> {
    return this.http.post<rule>(this.url, rule).pipe(
      tap(_ => Logger.log("rule was added"))
    );
  }

  updateRule(rule: rule): Observable<rule> {
     return this.http.put<rule>(this.url+ `/${rule.id}`, rule).pipe(
       tap(_ => Logger.log(`${rule.id} rule was updated`))
     );
  }


  deleteRule(id:number): Observable<rule> {
     return this.http.delete<rule>(this.url + `/${id}`).pipe(
       tap(_ => Logger.warn(`${id} rule was deleted`)),
      //  catchError(this.handleError('deleteHero'))
     );
  }

  // TODO: setup error handler
  // private handleError<rule> (operation = 'operation', result?: rule) {
  //   return (error: any): Observable<rule> => {
  //     Logger.error(error); 
  //     console.log(`${operation} failed: ${error.message}`); 
  //     return of(result as rule);   
  //   };
  // }

}
