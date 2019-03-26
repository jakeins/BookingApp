export class JwtToken {
  constructor(
    public accessToken: string,
    public refreshToken: string,
    public expireOn: Date
  ) { }
}
