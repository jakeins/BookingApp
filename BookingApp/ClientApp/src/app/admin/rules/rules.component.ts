import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RuleService } from '../../services/rule.service';
import { rule } from '../../models/rule';
import { DatePipe } from '@angular/common';


@Component({
  selector: 'app-rules',
  templateUrl: './rules.component.html',
  styleUrls: ['./rules.component.css'],
  providers: [DatePipe]
})
export class RulesComponent implements OnInit {
  rules: rule[];

  constructor(
    private http: HttpClient,
    private ruleService: RuleService,
    private datePipe: DatePipe
  ) { }

  ngOnInit() {
    this.getRules();
    this.datePipe = new DatePipe("en-US");
  }

  getRules() {
    this.ruleService.getRules().subscribe((rules: rule[]) => {
      this.rules = rules;
      console.log("Rules", this.rules);
    });
  }

  getDateTime(time: string) {
    console.log(this.datePipe.transform(time, 'dd/MM/yyyy, hh:mm:ss'));
    return this.datePipe.transform(time, 'dd/MM/yyyy, hh:mm:ss');
  }
}
