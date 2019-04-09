import { Component, OnInit, Inject } from '@angular/core';
import { RuleService } from '../../../services/rule.service';
import { FormGroup, Validators, FormBuilder} from '@angular/forms';
import { rule } from '../../../models/rule';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';



@Component({ 
  selector: 'app-rule',
  templateUrl: './rule.component.html',
  styleUrls: ['./rule.component.css']
})
export class RuleComponent implements OnInit {
  Rule: rule;
  form: FormGroup;
  numberPattern = "[0-9]{1,10}";
  isUpdate: boolean = false;
  constructor(private service: RuleService,
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<RuleComponent>,
    @Inject(MAT_DIALOG_DATA) public ruleId: number
    ) { }

  ngOnInit() {
    this.form = this.fb.group({
      id: null,
      title: ['', Validators.compose([Validators.required, Validators.minLength(4), Validators.maxLength(64), Validators.pattern("[a-zA-Z0-9 ]*")])],
      minTime: [0, Validators.compose([Validators.required, Validators.pattern(this.numberPattern)])],
      maxTime: [0, Validators.compose([Validators.required, Validators.pattern(this.numberPattern)])],
      serviceTime: [0, Validators.compose([Validators.required, Validators.pattern(this.numberPattern)])],
      stepTime: [0, Validators.compose([Validators.required, Validators.pattern(this.numberPattern)])],
      preOrderTimeLimit: [0, Validators.compose([Validators.required, Validators.pattern(this.numberPattern)])],
      reuseTimeout: [0, Validators.compose([Validators.required, Validators.pattern(this.numberPattern)])],
      isActive: false
    })
    if(this.ruleId != null){
       this.isUpdate = true;
        this.populateForm();
      }
    else
    {
      this.initializeForm();
      this.form.valueChanges.subscribe(console.log);
    }
  }
  
  populateForm(){
    this.service.getRule(this.ruleId).subscribe((res: rule) =>{
      this.form.setValue({
        id: res.id,
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

  onCreate(){
    if(this.form.valid){
      this.service.addRule(this.form.value).subscribe( res => {
        console.log('rule was created');
        this.onReset();
      });
      this.onClose()
      this.service.getRules();
    }
  }

  onSubmit(){
     if(this.form.valid){
      this.service.updateRule(this.form.value).subscribe( res => {
        console.log('rule was updated');
        this.onReset();
      });
      this.onClose();
      this.service.getRules();
     }
  }

  onReset(){          
    this.form.reset();
    this.initializeForm();
    Object.keys(this.form.controls).forEach(key => {                //reset form validation errors
        this.form.controls[key].setErrors(null)
      });
    this.form.clearValidators();
  }

  onClose(){
    this.onReset();
    this.dialogRef.close();
  }

  initializeForm(){
    this.form.setValue({
      id: null,
      title: [''],
      minTime: 0,
      maxTime: 0,
      serviceTime: 0,
      stepTime: 0,
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
  //#endregion

}
