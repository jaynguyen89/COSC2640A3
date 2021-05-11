import React from 'react';
import { connect } from 'react-redux';

import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import {defaultCredentials, ILoginComponent} from "./redux/interfaces";
import {
    EMPTY_STATUS,
    EMPTY_STRING,
    IStatusMessage,
    removeGlobalMessage,
    setGlobalMessage
} from "../../providers/helpers";
import {invokeAuthenticationRequest, setAuthUser} from "./redux/actions";
import * as authenticationConstants from './redux/constants';
import ReCAPTCHA from 'react-google-recaptcha';

const mapStateToProps = (state: any) => ({
    authentication: state.authenticationStore.authenticate,
    authUser: state.authenticationStore.authUser
});

const mapActionsToProps = {
    invokeAuthenticationRequest,
    setAuthUser
};

const Login = (props: ILoginComponent) => {
    const [credentials, setCredentials] = React.useState(defaultCredentials);
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);

    const [shouldEnableLoginButton, setShouldEnableLoginButton] = React.useState(true);

    const updateCredentials = (value: string | boolean, field: string) => {
        if (field === 'emailOrUsername') {
            if ((value as string).indexOf('@') !== -1) setCredentials({ ...credentials, email: value as string, username: EMPTY_STRING });
            else setCredentials({ ...credentials, username: value as string, email: EMPTY_STRING });
        }

        if (field === 'password') setCredentials({ ...credentials, password: value as string });
        if (field === 'role') setCredentials({ ...credentials, asStudent: !value as boolean });
        if (field === 'recaptcha') setCredentials({ ...credentials, recaptchaToken: value as string });
    }

    const attemptLogin = () => props.invokeAuthenticationRequest(credentials);

    React.useEffect(() => {
        const storedGlobalMessage = sessionStorage.getItem('globalMessage');
        if (storedGlobalMessage) {
            setStatusMessage(JSON.parse(storedGlobalMessage) as IStatusMessage);
            removeGlobalMessage();
        }
    }, []);

    React.useEffect(() => {
        setShouldEnableLoginButton(props.authentication.action !== authenticationConstants.AUTHENTICATION_BEGIN);

        if (props.authentication.action === authenticationConstants.AUTHENTICATION_FAILED)
            setStatusMessage({ messages: ['Unable to communicate with server: connection timed out.'], type: 'error' } as IStatusMessage);

        if (props.authentication.action === authenticationConstants.AUTHENTICATION_SUCCESS)
            if (props.authentication.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.authentication.payload.result === 0)
                setStatusMessage({ messages: props.authentication.payload.data, type: 'error' } as IStatusMessage);
            else {
                setStatusMessage(EMPTY_STATUS);

                if ((props.authentication.payload.data as any).shouldConfirmTfa) {
                    localStorage.setItem('TempAuth', JSON.stringify((props.authentication.payload.data as any).authenticatedUser));
                    window.location.href = '/confirm-tfa';
                }
                else {
                    props.setAuthUser((props.authentication.payload.data as any).authenticatedUser);
                    window.location.href = '/home';
                }
            }
    }, [props.authentication]);

    if (props.authUser.isAuthenticated)
        window.location.href = '/home';

    return (
        <div className='main-content'>
            <div className='login-area'>
                <Alert { ...statusMessage } />
                { props.authentication.action === authenticationConstants.AUTHENTICATION_BEGIN && <Spinner /> }

                <div className='card horizontal'>
                    <div className='card-content'>
                        <span className='card-title'>
                            <i className='material-icons teal-text'>fingerprint</i>
                            Login
                        </span>

                        <div className='row'>
                            <div className='input-field col s12'>
                                <i className='material-icons prefix'>account_circle</i>
                                <input id='emailOrUsername'
                                       value={ credentials.email || credentials.username }
                                       onChange={ e => updateCredentials(e.target.value, 'emailOrUsername') }
                                       onKeyDown={ e => { if (e.keyCode === 13) attemptLogin(); }}
                                       type='text'
                                       className='validate'
                                />
                                <label htmlFor='emailOrUsername'>Email or Username</label>
                            </div>

                            <div className='input-field col s12'>
                                <i className='material-icons prefix'>lock</i>
                                <input id='password'
                                       value={ credentials.password }
                                       onChange={ e => updateCredentials(e.target.value, 'password') }
                                       onKeyDown={ e => { if (e.keyCode === 13) attemptLogin(); }}
                                       type='password'
                                       className='validate'
                                />
                                <label htmlFor='password'>Password</label>
                            </div>

                            <div className="col s12 switch" style={{ marginBottom: '1.5em' }}>
                                <label>
                                    <span style={{ color: credentials.asStudent ? '#26a69a' : EMPTY_STRING }}>As Student</span>
                                    <input type="checkbox"
                                           value={ ((credentials.asStudent && EMPTY_STRING) || 'checked') as string }
                                           onChange={ e => updateCredentials(e.target.checked, 'role') }
                                    />

                                    <span className="lever" />
                                    <span style={{ color: credentials.asStudent ? EMPTY_STRING : '#26a69a' }}>As Teacher</span>
                                </label>
                            </div>

                            <div className='col s12' style={{ marginBottom: '1.5em' }}>
                                <ReCAPTCHA
                                    sitekey='6LeXhN4UAAAAAHKW6-44VxtUVMYSlMPj04WRoC8z'
                                    onChange={ val => updateCredentials(val as string, 'recaptcha') }
                                />
                            </div>

                            <div className='col s12'>
                                <button className={ shouldEnableLoginButton ? 'btn waves-effect waves-light' : 'btn disabled' }
                                        onClick={ attemptLogin }
                                >
                                    <i className='material-icons right'>done</i>
                                    Go
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <div className='row'>
                    <div className='col s12 center-align'>
                        Need to create an account? <a href='/register'>Register here</a>.
                    </div>
                    <div className='col s12 center-align'>
                        Forgot password? <a href='/forgot-password'>Recover here</a>.
                    </div>
                    <div className='col s12 center-align' style={{ marginTop: '10px' }}>
                        <a href='/statistics'>Click here</a> to view sales statistics (AWS MapReduce).
                    </div>
                </div>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(Login);