export default Object.assign(process.env, (window as any).env) as {
    REACT_APP_ENDPOINT: string,
    REACT_APP_SENTRY_DNS: string,
    REACT_APP_VERSION: string,
    REACT_APP_SENTRY_ENABLED: boolean
};