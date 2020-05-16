import React, { FC } from 'react';
import CssBaseline from '@material-ui/core/CssBaseline';
import { Switch, Route } from 'react-router-dom';
import { SnackbarProvider } from 'notistack';
import AccountIndex from 'modules/account';
import { IoCComponent } from 'components/ioc-register.component';
import { ThemeProvider } from '@material-ui/styles';
import DefaultTheme from 'themes/default';

const App: FC = () => {
  return (
    <ThemeProvider theme={DefaultTheme}>
      <SnackbarProvider maxSnack={3}>
        <IoCComponent>
          <CssBaseline />
          <Switch>
            <Route path="/account" component={AccountIndex} />
          </Switch>
        </IoCComponent>
      </SnackbarProvider>
    </ThemeProvider>);
}

export default App;