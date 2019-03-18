import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs/Observable';
import { tap } from 'rxjs/operators';
import { rule } from '../models/rule';
import { BASE_API_URL } from '../globals';

@Injectable()
export class RuleService {

  url: string = BASE_API_URL + '/rules';
  constructor(private http: HttpClient) { }

  getRule(id: number): Observable<rule> {
    return this.http.get<rule>(this.url + '/' + id);
  }

  getRules(): Observable<rule[]> {
    return this.http.get<rule[]>(this.url).pipe(
      tap(_ => console.log("rules were loaded"))
    );
  }

  saveRule(rule: rule) {
    return this.http.put(this.url, rule).subscribe();
  }


  deleteRule(rule: rule): Observable<rule> {
    return this.http.delete<rule>(this.url);
  }

  addRule(rule: rule): Observable<rule> {
    return this.http.post<rule>(this.url, rule);
  }
}
