
export interface ISignInRequest {
    email: string;
    password: string;
    remember: boolean;

}

export interface ISignInResponse {
    access_token: string;
    refresh_token: string;
}
