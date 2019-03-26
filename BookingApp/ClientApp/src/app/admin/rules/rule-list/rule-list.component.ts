import { Component, OnInit } from '@angular/core';
import { RuleService } from '../../../services/rule.service';
import { rule } from '../../../models/rule';
import { Logger } from '../../../services/logger.service';

@Component({
  selector: 'app-rule-list',
  templateUrl: './rule-list.component.html',
  styleUrls: ['./rule-list.component.css']
})
export class RuleListComponent implements OnInit {
  error: string;
  selectedRow: number;
  constructor(
    private service: RuleService
  ) { }

  ngOnInit() {
    this.service.refreshList();
  }

  populateForm(rule: rule, i: number){
    this.service.Rule = Object.assign({}, rule);
    this.selectedRow = i;
    this.service.showAdditionalInfo = true;
  }

  onDelete(id:number){
    if(confirm('Are you sure to delete rule?')){
    this.service.deleteRule(id).subscribe( res => {
      this.service.showAdditionalInfo = false;
      this.service.refreshList();
    },
    error => { 
      this.error = error['status'] + ': ' + error['error']['Message'];
      Logger.error(error)
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
}
