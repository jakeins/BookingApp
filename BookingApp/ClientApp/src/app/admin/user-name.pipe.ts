import { Pipe, PipeTransform } from '@angular/core';
import { UserService } from '../services/user.service';
import { User } from '../models/user';

@Pipe({
  name: 'userName'
})
export class UserNamePipe implements PipeTransform {
  //public user: User = new User();
  static user: User; //TODO: fix temporary bug, that return undefined on the first loading only
  constructor(
    private userSerive: UserService
  ){}
  transform(value: string) {
     this.userSerive.getUserById(value).subscribe(((res:User) => {
        UserNamePipe.user = Object.assign({}, res);
        console.log(UserNamePipe.user.userName);
      })
    )
     return UserNamePipe.user.userName;
  }
}
