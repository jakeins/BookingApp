export class RegisterFormModel {
    constructor(
        public userName: string,
        public email: string,
        public password: string,
        public confirmPassword: string
    ) {
    }
}
