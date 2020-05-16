import { ISignInResponse, ISignInRequest } from "./account.interfaces";
import { inject } from "aurelia-dependency-injection";
import { AxiosInstance } from "axios";

@inject('axios')
export class AccountGateway {

    constructor(
        private axios: AxiosInstance
    ) {

    }

    async signIn(request: ISignInRequest): Promise<ISignInResponse> {
        const response = await this.axios.post('api/account/sign-in', request);
        return response.data;
    }

    async refreshToken(dto: { access_token: string, refresh_token: string }): Promise<ISignInResponse> {
        const response = await this.axios.post('api/account/refresh-token', dto);
        return response.data;
    }

}