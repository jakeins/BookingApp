import { AuthLogin } from "./auth-login";

export class UserRegister extends AuthLogin {
  userName: string;
  confirmPassword: string;
}
