import { Component, OnInit } from '@angular/core';
import { RuleService } from '../../../services/rule.service';
import { NgForm } from '@angular/forms';
import { rule } from '../../../models/rule';

@Component({
  selector: 'app-rule',
  templateUrl: './rule.component.html',
  styleUrls: ['./rule.component.css']
})
export class RuleComponent implements OnInit {
  ruleToCheck: rule;
  showAdditionalInfo = false;
  
  constructor(
    private service: RuleService
  ) { }

  ngOnInit() {
    this.resetForm();
  }

  onSubmit(form: NgForm){
    if(form.value.id == null)
      this.insertRecord(form);
    else
      this.updateRecord(form);
  }

  onReset(form: NgForm){
    this.resetForm(form);
  }
  
  insertRecord(form: NgForm){
    this.service.addRule(form.value).subscribe(res =>{
      console.log("rule was added");
      this.resetForm();
      this.service.refreshList();
    })
  }

  updateRecord(form: NgForm){
    this.service.updateRule(form.value).subscribe(res =>{
      console.log("rule was updated");
      this.resetForm();
      this.service.refreshList();
    })
  }

  resetForm(form?: NgForm){
    // if(form !=null)
    //  form.resetForm();
    this.service.Rule = {
      title: 'title',
      minTime:10,
      maxTime: 100,
      stepTime: 0,
      serviceTime: 0,
      preOrderTimeLimit: 0,
      reuseTimeout: 0,
      isActive: false
    }
  }
}
