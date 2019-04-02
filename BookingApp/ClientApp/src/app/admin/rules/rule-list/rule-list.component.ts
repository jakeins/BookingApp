import { Component, OnInit, Input } from '@angular/core';
import { RuleService } from '../../../services/rule.service';
import { rule } from '../../../models/rule';
import { Logger } from '../../../services/logger.service';

@Component({
  selector: 'app-rule-list',
  templateUrl: './rule-list.component.html',
  styleUrls: ['./rule-list.component.css']
})
export class RuleListComponent implements OnInit {
  @Input() ruleId: number;
  error: string;
  selectedRow: number;
  constructor(
    private service: RuleService
  ) { }

  ngOnInit() {
    this.service.refreshList();
    if(this.ruleId != null){
       this.populateForm(this.ruleId);
    }
  }

  populateForm(i: number, rule?: rule ){
    if(rule != null){
      this.service.Rule = Object.assign({}, rule);
      this.service.listSelectedRow = i;
    }  
    else
      this.service.getRule(i).subscribe((res:rule)=>{
        this.service.Rule = res;
        this.service.listSelectedRow = i-1;
      });

    this.service.showAdditionalInfo = true;
  }

  onDelete(id:number){
    if(confirm('Are you sure to delete rule?')){
    this.service.deleteRule(id).subscribe( res => {
      this.service.showAdditionalInfo = false;
      this.service.refreshList();
    },
    err => { 
      this.error = err.status + ': ' + err.error.Message;
    })
    }
  }

  showActive(isActive: boolean):string{
    if(isActive)
      return 'active';
    else
      return 'not active';
  }

  onReset(){
    this.error = null;
  }

  ngOnDestroy(){
    this.ruleId = null;
    this.service.showAdditionalInfo = false;
    this.service.listSelectedRow= null;
  }
}
