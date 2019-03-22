import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs/Observable';
import { tap } from 'rxjs/operators';
import { rule } from '../models/rule';
import { BASE_API_URL } from '../globals';

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
    this.http.get(this.url).toPromise().then((res: rule[]) => {
      console.log("list was refreshed");
      this.Rules = res; 
    });
  }

  getRule(id: number): Observable<rule> {
    return this.http.get<rule>(this.url + '/' + id);
  }

  getRules(): Observable<rule[]> {
    return this.http.get<rule[]>(this.url).pipe(
      tap(_ => console.log("rules were loaded"))
    );
  }

  addRule(rule: rule): Observable<rule> {
    return this.http.post<rule>(this.url, rule).pipe(
      tap(_ => console.log("rulee was added"))
    );
  }

  updateRule(rule: rule) {
     return this.http.put(this.url+ `/${rule.id}`, rule);
  }


  deleteRule(id:number){
     return this.http.delete<rule>(this.url + `/${id}`);
  }


}
