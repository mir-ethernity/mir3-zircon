import { AxiosResponse, AxiosRequestConfig, AxiosInstance } from "axios";
import { WithSnackbarProps } from "notistack";
import { inject } from "aurelia-dependency-injection";
import { AccountGateway } from "../modules/account/account.gateway";
import i18next from "i18next";


@inject('snackbar', AccountGateway)
export class GatewayUtils {

    constructor(
        private snackbar: WithSnackbarProps,
        private accountGateway: AccountGateway,
    ) {

    }

    setAuthToken(config: AxiosRequestConfig) {
        const accessToken = localStorage.getItem('auth_access_token');
        if (accessToken !== null) {
            config.headers.Authorization = 'Bearer ' + accessToken;
        }
    }

    async tryRefreshToken(axios: AxiosInstance, response: AxiosResponse, config: AxiosRequestConfig) {
        const rqHeaders = config.headers;

        const access_token: string = rqHeaders && rqHeaders.Authorization ? rqHeaders.Authorization.split(' ')[1] : null;
        const refresh_token = localStorage.getItem('auth_refresh_token');

        try {
            if (access_token && refresh_token) {
                const result = await this.accountGateway.refreshToken({ access_token, refresh_token });
                localStorage.setItem('auth_access_token', result.access_token);
                config.headers.Authorization = result.access_token;
                return await axios.request(config);
            }
        } catch (e) {
            throw e;
        }

        return response;
    }

    printNetworkError() {
        const message = i18next.t('axios.network_error');
        this.snackbar.enqueueSnackbar(message, {
            variant: 'error',
            style: { whiteSpace: 'pre-line' },
        });
    }

    printBadRequestMessage(response: AxiosResponse) {
        let message: string | null = null;
        if (response.data.message) {
            if (typeof response.data.message === 'string') {
                message = response.data.message;
            } else if (response.data.message instanceof Array) {
                const messages: string[] = [];
                for (const rule of response.data.message) {
                    for (const constraintType in rule.constraints) {
                        const constraintMessage = rule.constraints[constraintType];
                        messages.push(constraintMessage);
                    }
                }
                message = messages.join('\n');
            }
        } else if (response.data.error) {
            message = response.data.error;
        } else {
            message = 'La petición no es correcta';
        }

        if (message !== null) {
            this.snackbar.enqueueSnackbar(message, {
                variant: 'error',
                style: { whiteSpace: 'pre-line' },
            });
        }
    }

}
