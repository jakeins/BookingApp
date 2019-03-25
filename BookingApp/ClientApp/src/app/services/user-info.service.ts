import * as jwt_decode from 'jwt-decode';
import { Injectable } from '@angular/core';
import { TokenService } from './token.service';
import { Logger } from './logger.service';

@Injectable()
export class UserInfoService {
    private roleKey = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

    constructor(private tokenService: TokenService) {

    }

    private decodeToken() {
        const jwt = this.tokenService.readJwtToken();
        let rawToken = null;
        if (jwt != null) {
            rawToken = jwt.accessToken;
        }

        let decodedToken = null;

        if (rawToken != null) {
            try {
                decodedToken = jwt_decode(rawToken);
            } catch (e) { Logger.error('Cannot decode access token'); }
        }
        return decodedToken;
    }

    public get userId(): string {
        if (this.decodeToken() == null) {
            return null;
        } else {
            return this.decodeToken().uid;
        }
    }

    public get username(): string {
        if (this.decodeToken() == null) {
            return null;
        } else {
            return this.decodeToken().sub;
        }
    }

    public get email(): string {
        if (this.decodeToken() == null) {
            return null;
        } else {
            return this.decodeToken().email;
        }
    }

    public get roles(): Array<string> {
        if (this.decodeToken() == null) {
            return null;
        } else {
            let roles = this.decodeToken()[this.roleKey];

            if (!(roles instanceof Array)) {
                roles = [roles];
            }

            return roles;
        }
    }
}
