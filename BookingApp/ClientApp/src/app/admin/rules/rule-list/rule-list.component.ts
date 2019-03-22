import { Component, OnInit } from '@angular/core';
import { RuleService } from '../../../services/rule.service';
import { rule } from '../../../models/rule';

@Component({
  selector: 'app-rule-list',
  templateUrl: './rule-list.component.html',
  styleUrls: ['./rule-list.component.css']
})
export class RuleListComponent implements OnInit {
  constructor(
    private service: RuleService
  ) { }

  ngOnInit() {
    this.service.refreshList();
  }

  populateForm(rule: rule){
    this.service.showAdditionalInfo = true;
    this.service.Rule = Object.assign({}, rule);
    
  }

  onDelete(id:number){
    if(confirm('Are you sure to delete rule?')){
    this.service.deleteRule(id).subscribe( res => {
      console.log("rule was deleted");
      this.service.showAdditionalInfo=false;
      this.service.refreshList();
    });
    }
  }

  showActive(isActive: boolean):string{
    if(isActive)
      return 'active';
    else
      return 'not active';
  }
}
