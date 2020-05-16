import Axios, { AxiosError } from "axios"
import { Container } from "aurelia-dependency-injection";
import config from "../config";
import { GatewayUtils } from "./gateway.utils";

import * as Sentry from '@sentry/browser';

const registerAxiosAPI = (container: Container) => {

    // TODO: Hay que registrar axios como una factira, ahora mismo peta...

    container.registerHandler('axios', () => {
        const instance = Axios.create({
            baseURL: config.REACT_APP_ENDPOINT
        });

        instance.interceptors.request.use((config) => {
            const gatewayUtils = container.get(GatewayUtils);
            gatewayUtils.setAuthToken(config);
            return config;
        });

        instance.interceptors.response.use((response) => {
            return response;
        }, async (error: AxiosError) => {
            const gatewayUtils = container.get(GatewayUtils);

            const { response, config } = error;
            debugger;
            if (response) {
                if (response.status === 401) {
                    return gatewayUtils.tryRefreshToken(instance, response, config);
                } else if (response.status === 400) {
                    gatewayUtils.printBadRequestMessage(response);
                } else if (response.status === 500) {
                    Sentry.captureException(error);
                }
            } else {
                gatewayUtils.printNetworkError();
            }
            throw error;
        });

        return instance;
    });

    container.registerSingleton(GatewayUtils);
}

export default registerAxiosAPI;
