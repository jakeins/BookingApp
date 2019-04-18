import { Component, OnInit } from '@angular/core';
import { Input } from '@angular/core';
import { RuleService } from '../../services/rule.service';
import { rule } from '../../models/rule';
import { UserInfoService } from '../../services/user-info.service';

@Component({
  selector: 'app-rule',
  templateUrl: './rule.component.html',
  styleUrls: ['./rule.component.css']
})
export class SiteRuleComponent implements OnInit {
  @Input() ruleId: number;
  isAdmin:boolean  = false;
  Rule: rule;
  constructor(
    private ruleService: RuleService,
    private userInfoService: UserInfoService
  ) { }

  ngOnInit() {
    this.isAdmin = this.userInfoService.isAdmin;
    this.onReset();
  }

  onReset(){
    this.ruleService.getRule(this.ruleId).subscribe((res: rule)=>{
      this.Rule = res;
    });
  }

  showTime(a:number):string{
    if(a > 1)
      if(a < 60)
        return `${a} minutes`;
      else
       {
         let hours = Math.floor(a/60);
         let minutes = a - (hours* 60);
         let str = `${hours} hours`;
          if(minutes > 0)
            if( minutes > 2)
              str += `, ${minutes} minutes`;
            else
              str += `, ${minutes} minute`;
         return str;
       }
    else
      return `${a} minute`;
  }
}