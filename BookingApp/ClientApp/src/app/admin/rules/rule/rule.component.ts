import { Component, OnInit, Inject } from '@angular/core';
import { RuleService } from '../../../services/rule.service';
import { FormGroup, Validators, FormBuilder} from '@angular/forms';
import { rule } from '../../../models/rule';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { USERNAME_REGEX } from '../../../globals';
import { NotificationService } from '../../../services/notification.service';
import { Router } from '@angular/router';



@Component({ 
  selector: 'app-rule',
  templateUrl: './rule.component.html',
  styleUrls: ['./rule.component.css']
})
export class RuleComponent implements OnInit {
  Rule: rule;
  form: FormGroup;
  numberPattern = "[0-9]{1,5}";
  isUpdate: boolean = false;
  isReadonly: boolean = false;
  error: string;
  constructor(
    private service: RuleService,
    private notificationService: NotificationService,
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<RuleComponent>,
    @Inject(MAT_DIALOG_DATA) public ruleData: any,
    private router: Router
        ) { }

  ngOnInit() {
    this.createForm();
    if(this.ruleData!= null){
          if(this.ruleData['readonlymode'] !=null)
          {
            this.createReadOnlyForm();
            this.isReadonly = true;
            this.populateReadonlyForm();
          }
          else
          {
            this.isUpdate = true;
            this.populateForm();
          }
    } 
    else
    {
      this.initializeForm();
    }
  }
  
  createForm(){
    this.form = this.fb.group({
      id: null,
      title: ['', Validators.compose([Validators.required, Validators.minLength(4), Validators.maxLength(64), Validators.pattern(USERNAME_REGEX)])],
      minTime: [0, Validators.compose([Validators.required, Validators.min(1), Validators.max(14400),Validators.pattern(this.numberPattern)])],
      maxTime: [0, Validators.compose([Validators.required, Validators.min(1), Validators.max(14400), Validators.pattern(this.numberPattern)])],
      serviceTime: [0, Validators.compose([Validators.required, Validators.min(0), Validators.max(14400), Validators.pattern(this.numberPattern)])],
      stepTime: [1, Validators.compose([Validators.required, Validators.min(1), Validators.max(14400), Validators.pattern(this.numberPattern)])],
      preOrderTimeLimit: [0, Validators.compose([Validators.required, Validators.min(0), Validators.max(14400),Validators.pattern(this.numberPattern)])],
      reuseTimeout: [0, Validators.compose([Validators.required, Validators.min(0), Validators.max(14400), Validators.pattern(this.numberPattern)])],
      isActive: false
    })
  }

  createReadOnlyForm(){
    this.form = this.fb.group({
      id: null,
      title: ['', Validators.compose([Validators.required, Validators.minLength(4), Validators.maxLength(64), Validators.pattern(USERNAME_REGEX)])],
      minTime: [0, Validators.compose([Validators.required, Validators.pattern(this.numberPattern)])],
      maxTime: [0, Validators.compose([Validators.required, Validators.pattern(this.numberPattern)])],
      serviceTime: [0, Validators.compose([Validators.required, Validators.pattern(this.numberPattern)])],
      stepTime: [1, Validators.compose([Validators.required, Validators.pattern("[1-9]{1,10}")])],
      preOrderTimeLimit: [0, Validators.compose([Validators.required, Validators.pattern(this.numberPattern)])],
      reuseTimeout: [0, Validators.compose([Validators.required, Validators.pattern(this.numberPattern)])],
      isActive: false,
      createdTime: null,
      updatedTime: null,
      createdUserId: 'ed7e3433-b6b4-488b-b547-9eb880cb316c', 
      updatedUserId: 'ed7e3433-b6b4-488b-b547-9eb880cb316c'
    })
  }

  populateForm(){
    this.service.getRule(this.ruleData['ruleId']).subscribe((res: rule) =>{
      this.form.setValue({
        id: [res.id],
        title: res.title,
        minTime: res.minTime,
        maxTime: res.maxTime,
        serviceTime: res.serviceTime,
        stepTime: res.stepTime,
        preOrderTimeLimit: res.preOrderTimeLimit,
        reuseTimeout: res.reuseTimeout,
        isActive: res.isActive 
      });
    });
  }

  populateReadonlyForm(){
    this.service.getRule(this.ruleData['ruleId']).subscribe((res: rule) =>{
      this.form.setValue({
        id: res.id,
        title: res.title,
        minTime: res.minTime,
        maxTime: res.maxTime,
        serviceTime: res.serviceTime,
        stepTime: res.stepTime,
        preOrderTimeLimit: res.preOrderTimeLimit,
        reuseTimeout: res.reuseTimeout,
        isActive: res.isActive,
        createdTime: res.createdTime,
        updatedTime: res.updatedTime,
        createdUserId: res.createdUserId,
        updatedUserId: res.updatedUserId
      });
      this.title.disable();
      this.isActive.disable();
      this.maxTime.disable();
      this.minTime.disable();
      this.serviceTime.disable();
      this.step.disable();
      this.preOrder.disable();
      this.reuse.disable();
      this.createdTime.disable();
      this.updatedTime.disable();
      this.createdUserId.disable();
      this.updatedUserId.disable();
    });
  }

  onCreate(){
    if(this.form.valid){
      this.service.addRule(this.form.value).subscribe(
        () => {
          this.onClose();
          this.notificationService.success('Created successfully!');
        },
        err => { 
          this.error =  'Error ' + err.status + ': ' + err.error.Message + '.';
        });
    }
  }

  onSubmit(){
     if(this.form.valid){
      this.service.updateRule(this.form.value).subscribe(
        () => {
          this.onClose();
          this.notificationService.submit('Submitted successfully!');
        },
        err => { 
          this.error =  'Error ' + err.status + ': ' + err.error.Message + '.';
        });
     }
  }

  onReset(){          
    this.form.reset();
    this.initializeForm();
    // Object.keys(this.form.controls).forEach(key => {                //reset form validation errors
    //     this.form.controls[key].setErrors(null)
    //   });
  }

  onClose(){
    if(!this.isReadonly)
      this.onReset();
    this.dialogRef.close();
  }

  onClear(){
    this.error = null;
  }

  initializeForm(){
    this.form.setValue({
      id: null,
      title: [''],
      minTime: 0,
      maxTime: 0,
      serviceTime: 0,
      stepTime: 1,
      preOrderTimeLimit: 0,
      reuseTimeout: 0,
      isActive: false 
    });
  }

  //#region form utils
  get title(){
    return this.form.get('title');
  }
  get minTime(){
    return this.form.get('minTime');
  }
  get maxTime(){
    return this.form.get('maxTime');
  }
  get serviceTime(){
    return this.form.get('serviceTime');
  }
  get step(){
    return this.form.get('stepTime');
  }
  get reuse(){
    return this.form.get('reuseTimeout');
  }
  get preOrder(){
    return this.form.get('preOrderTimeLimit');
  }
  get isActive(){
    return this.form.get('isActive');
  }
  get createdTime(){
    return this.form.get('createdTime');
  }
  get createdTimeValue(){
    return this.form.get('createdTime').value;
  }
  get updatedTime(){
    return this.form.get('updatedTime');
  }
  get updatedTimeValue(){
    return this.form.get('updatedTime').value;
  }
  get createdUserId(){
    return this.form.get('createdUserId');
  }
  get createdUserIdValue(){
    return this.form.get('createdUserId').value;
  }
  get updatedUserId(){
    return this.form.get('updatedUserId');
  }
  get updatedUserIdValue(){
    return this.form.get('updatedUserId').value;
  }
  //#endregion

}
