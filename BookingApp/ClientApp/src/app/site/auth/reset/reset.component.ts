import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'app-reset',
  templateUrl: './reset.component.html'
})
export class ResetComponent implements OnInit, OnDestroy {

  private querySubscription: Subscription;
  private resetForm: FormGroup;
  private code: string;
  private userId: string;
  private isParamsExist = true;

  constructor(private authService: AuthService, private route: ActivatedRoute,
    private router: Router) {
    this.resetForm = new FormGroup({
      'password': new FormControl('', Validators.required),
      'confirmPassword': new FormControl(''),
    }, { validators: this.comparePassword });
    this.querySubscription = route.queryParams.subscribe(
      (queryParam: any) => {
        this.userId = queryParam['userId'];
        this.code = queryParam['code'];
      }
    );
  }

  ngOnInit() {
    if (this.userId !== undefined || this.code !== undefined) {
      this.isParamsExist = true;
    } else {
      this.isParamsExist = false;
    }
  }

  ngOnDestroy() {
    this.querySubscription.unsubscribe();
  }

  reset() {
    this.router.navigate(['/login']);
  }

  private comparePassword(group: FormGroup) {
    const pass = group.value.password;
    const confirm = group.value.confirmPassword;

    return pass === confirm ? null : { notSame: true };
  }

}
