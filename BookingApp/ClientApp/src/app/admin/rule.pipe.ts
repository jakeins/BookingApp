import { Pipe, PipeTransform, Injector } from '@angular/core';
import { UserService } from '../services/user.service';
import { User } from '../models/user';
import { Observable } from 'rxjs';
import { Logger } from '../services/logger.service';

@Pipe({
  name: 'userName'
})
export class UserNamePipe implements PipeTransform {
  user: User ;  

  constructor(
    private injector: Injector
  ){}
  transform(id: string): Observable<string>{
    let userSerive = this.injector.get(UserService);
    return userSerive.getUserById(id).map(
      data => {
        this.user = data;
        return this.user.userName;
      },
      error => Logger.error('transform userId to userName')
      )
  }
}

@Pipe({
  name: 'activity'
})
export class RuleActivityPipe implements PipeTransform{
  constructor(){}

  transform(isActive: boolean):string{
    if(isActive)
      return "active";
    else
      return "not active";
  }
}
