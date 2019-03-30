import { Pipe, PipeTransform } from '@angular/core';
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
    private userSerive: UserService
  ){}
  transform(id:string):Observable<string>{
    return this.userSerive.getUserById(id).map(
      data => {
        this.user = data;
        return this.user.userName;
      },
      error => Logger.error('transform userId to userName')
      )
  }
}
