import * as jwt_decode from 'jwt-decode';
import { Injectable } from '@angular/core';

import { Logger } from './logger.service';

@Injectable()
export class UserInfoService {
    

    constructor() {

    }



  uID = 'uID';
  uName = 'uName';
  uEmail = 'uEmail';
  private roleKey = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
  uAdmin = 'uAdmin';


    public SaveUserInfo(decodedAT : any) {
      localStorage.setItem(this.uID, decodedAT.uid);
      localStorage.setItem(this.uName, decodedAT.sub);
      localStorage.setItem(this.uEmail, decodedAT.email);

      //admin detection
      let roles = decodedAT[this.roleKey];

      if (!(roles instanceof Array)) {
        roles = [roles];
      }

      let tempAdmin = false;

      for (let role of roles) {
          if (role == 'Admin') {
            tempAdmin = true;
            break;
          }
      }

      localStorage.setItem(this.uAdmin, String(tempAdmin));
  }

    public DeleteUserInfo() {
      localStorage.removeItem(this.uID);
      localStorage.removeItem(this.uName);
      localStorage.removeItem(this.uEmail);
      localStorage.removeItem(this.uAdmin);
    }



    public get userId(): string {
      return localStorage.getItem(this.uID);
    }

    public get username(): string {
      return localStorage.getItem(this.uName);
    }

    public get email(): string {
      return localStorage.getItem(this.uEmail);
    }

    public get isAdmin(): boolean {
      let info = localStorage.getItem(this.uAdmin);

      let result: boolean;

      if (info == 'true')
        result = true;
      else if (info == 'false')
        result = false;

      return result;
  }

  public get isUser(): boolean {
    return localStorage.getItem(this.uAdmin) != undefined;
  }


}
