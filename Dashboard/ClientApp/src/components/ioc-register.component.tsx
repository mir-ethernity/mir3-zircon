import React, { FC, useMemo, useEffect } from "react";
import registerAxiosAPI from '../common/api-axios.builder';
import { Container } from 'aurelia-dependency-injection';
import IoCContext from '../common/ioc.context';
import { useSnackbar } from "notistack";

export const IoCComponent: FC = ({ children }) => {

    const snackbar = useSnackbar();

    const container = useMemo(() => {
        const c = new Container();
        c.makeGlobal();
        return c;
    }, []);

    useEffect(() => {
        container.registerInstance('snackbar', snackbar);
        registerAxiosAPI(container);
    }, [container, snackbar]);

    return (<IoCContext.Provider value={container}>
        {children}
    </IoCContext.Provider>);
}