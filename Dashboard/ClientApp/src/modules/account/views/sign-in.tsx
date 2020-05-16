import React, { FC, useState, useContext } from "react"
import { Container, makeStyles, Avatar, Typography, TextField, FormControlLabel, Button, Box, Checkbox } from "@material-ui/core";
import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import Copyright from "../../../components/copyright";
import { AccountGateway } from "../account.gateway";
import IoCContext from "../../../common/ioc.context";
import { useHistory } from "react-router-dom";
import { useTranslation } from "react-i18next";

const useStyles = makeStyles(theme => ({
    root: {
        height: '100%',
        width: '100%',
    },
    paper: {
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
    },
    avatar: {
        margin: theme.spacing(1),
        backgroundColor: theme.palette.secondary.main,
    },
    form: {
        width: '100%', // Fix IE 11 issue.
        marginTop: theme.spacing(1),
    },
    submit: {
        margin: theme.spacing(3, 0, 2),
    },
}));

const SignInIndex: FC = () => {
    const classes = useStyles();
    const container = useContext(IoCContext);
    const history = useHistory();
    const { t } = useTranslation();

    const [loading, setLoading] = useState(false);
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [remember, setRemember] = useState(false);

    const handleSignIn = async (event: React.FormEvent) => {
        event.preventDefault();
        setLoading(true);

        try {
            const accountGateway = container.get(AccountGateway);

            const response = await accountGateway.signIn({
                email,
                password,
                remember,
            });

            localStorage.setItem('auth_access_token', response.access_token);
            localStorage.setItem('auth_refresh_token', response.refresh_token);

            history.push('/');
        } finally {
            setLoading(false);
        }
    }

    const handleChange = (callback: (value: any) => void, targetProperty: 'value' | 'checked') => {
        const handle: React.ChangeEventHandler<any> = (e) => {
            callback(e.target[targetProperty]);
        };
        return handle;
    }

    return (
        <div className={classes.root}>
            <Container component="main" maxWidth="xs">
                <div className={classes.paper}>
                    <Avatar className={classes.avatar}>
                        <LockOutlinedIcon />
                    </Avatar>
                    <Typography component="h1" variant="h5">
                        {t('account.sign_in')}
                    </Typography>
                    <form className={classes.form} noValidate onSubmit={handleSignIn}>
                        <TextField
                            variant="outlined"
                            margin="normal"
                            required
                            fullWidth
                            id="email"
                            label={t('account.email')}
                            name="email"
                            type="email"
                            autoComplete="email"
                            disabled={loading}
                            autoFocus
                            onChange={handleChange(setEmail, 'value')}
                        />
                        <TextField
                            variant="outlined"
                            margin="normal"
                            required
                            fullWidth
                            name="password"
                            label={t('account.password')}
                            type="password"
                            id="password"
                            disabled={loading}
                            autoComplete="current-password"
                            onChange={handleChange(setPassword, 'value')}
                        />
                        <FormControlLabel
                            control={<Checkbox value="remember" color="primary" />}
                            label={t('account.remember_me')}
                            disabled={loading}
                            onChange={handleChange(setRemember, 'checked')}
                        />
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            color="primary"
                            disabled={loading}
                            className={classes.submit}
                        >
                            {t('account.login')}
                        </Button>
                    </form>
                </div>
                <Box mt={8}>
                    <Copyright />
                </Box>
            </Container>
        </div>
    )
}

export default SignInIndex;
