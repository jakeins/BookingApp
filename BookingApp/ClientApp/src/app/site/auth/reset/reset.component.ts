import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { UserService } from '../../../services/user.service';
import { UserNewPassword } from '../../../models/user-new-password';


@Component({
  selector: 'app-reset',
  styleUrls: ['../form.auth.css'],
  templateUrl: './reset.component.html'
})
export class ResetComponent implements OnInit, OnDestroy {

  private querySubscription: Subscription;
  private resetForm: FormGroup;
  private code: string;
  private userId: string;
  private isParamsExist = true;
  private userPass: UserNewPassword = new UserNewPassword();

  constructor(private authService: AuthService, private route: ActivatedRoute,
    private router: Router, private userService: UserService) {
    this.resetForm = new FormGroup({
      'password': new FormControl('', Validators.required),
      'confirmPassword': new FormControl('', Validators.required),
    }, { validators: this.comparePassword });
    this.querySubscription = route.queryParams.subscribe(
      (queryParam: any) => {
        this.userId = queryParam['userId'];
        this.code = encodeURIComponent(queryParam['code']);
      }
    );
  }

  ngOnInit() {
    if (this.userId !== undefined && this.code !== undefined) {
      this.isParamsExist = true;
    } else {
      this.isParamsExist = false;
    }  
  }

  ngOnDestroy() {
    this.querySubscription.unsubscribe();
  }

  reset() {
    this.userPass.NewPassword = this.resetForm.value.password;
    this.userPass.ConfirmNewPassword = this.resetForm.value.confirmPassword;
    this.userService.ressetPassword(this.userId, this.code, this.userPass).subscribe(() => {
      this.router.navigate(['/login']);
    });
    
  }

  private comparePassword(group: FormGroup) {
    const pass = group.value.password;
    const confirm = group.value.confirmPassword;
    return pass === confirm ? null : { notSame: true };
  }

}
