import React, { FC } from "react";
import { Box, makeStyles } from "@material-ui/core";
import { Switch, Route, useRouteMatch } from "react-router-dom";
import SignInIndex from "./views/sign-in";

const useStyles = makeStyles(() => ({
    root: {
        width: '100%',
        height: '100%',
    }
}));

const AccountIndex: FC = () => {
    const match = useRouteMatch();
    const classes = useStyles();

    return (
        <div className={classes.root}>
            <Switch>
                <Route path={`${match.path}/sign-in`} component={SignInIndex} />
            </Switch>
        </div>
    )
}

export default AccountIndex;