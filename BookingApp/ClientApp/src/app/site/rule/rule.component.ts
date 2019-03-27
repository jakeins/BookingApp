import { Component, OnInit } from '@angular/core';
import { Input } from '@angular/core';
import { RuleService } from '../../services/rule.service';
import { rule } from '../../models/rule';

@Component({
  selector: 'app-rule',
  templateUrl: './rule.component.html',
  styleUrls: ['./rule.component.css']
})
export class RuleComponent implements OnInit {
  @Input() ruleId: number;

  Rule: rule;
  constructor(
    private ruleService: RuleService
  ) { }

  ngOnInit() {
    this.onReset();
  }

  onReset(){
    this.ruleService.getRule(this.ruleId).subscribe((res: rule)=>{
      this.Rule = res;
      //console.log(this.Rule.title);
    });
  }

}
