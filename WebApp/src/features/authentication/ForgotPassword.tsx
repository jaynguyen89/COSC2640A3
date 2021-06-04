import React from 'react';
import {connect} from 'react-redux';

import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import * as authenticationConstants from "./redux/constants";
import {EMPTY_STATUS, EMPTY_STRING, IStatusMessage, setGlobalMessage} from "../../providers/helpers";
import ReCAPTCHA from "react-google-recaptcha";
import {defaultIdentity, IForgotPassword} from "./redux/interfaces";
import {invokeForgotPasswordRequest} from "./redux/actions";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    forgotPassword: state.authenticationStore.forgotPassword
});

const mapActionsToProps = {
    invokeForgotPasswordRequest
};

const ForgotPassword = (props: IForgotPassword) => {
    const [identity, setIdentity] = React.useState(defaultIdentity);
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);

    const updateIdentity = (value: string, field: string) => {
        if (field === 'emailOrUsername') {
            if ((value as string).indexOf('@') !== -1) setIdentity({ ...identity, email: value as string, username: EMPTY_STRING });
            else setIdentity({ ...identity, username: value as string, email: EMPTY_STRING });
        }

        if (field === 'recaptcha') setIdentity({ ...identity, recaptchaToken: value });
    }

    React.useEffect(() => {
        const shouldSendRequest = (identity.email.length > 0 || identity.username.length > 0) && identity.recaptchaToken.length > 0;
        if (shouldSendRequest) props.invokeForgotPasswordRequest(identity);
    }, [identity]);

    React.useEffect(() => {
        if (props.forgotPassword.action === authenticationConstants.FORGOT_PASSWORD_REQUEST_FAILED)
            setStatusMessage({ messages: ['Unable to communicate with server: connection timed out.'], type: 'error' } as IStatusMessage);

        if (props.forgotPassword.action === authenticationConstants.FORGOT_PASSWORD_REQUEST_SUCCESS)
            if (props.forgotPassword.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.forgotPassword.payload.result === 0)
                setStatusMessage({ messages: props.forgotPassword.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setGlobalMessage({ messages: ['An email and/or SMS has been sent to you to let you reset your password.'], type: 'success' } as IStatusMessage);
                localStorage.setItem('recoverPassword_accountId', props.forgotPassword.payload.data as unknown as string);
                window.location.href = '/recover';
            }
    }, [props.forgotPassword]);

    if (props.authUser.isAuthenticated)
        window.location.href = '/home';

    return (
        <div className='main-content'>
            <div className='row'>
                { props.forgotPassword.action === authenticationConstants.FORGOT_PASSWORD_REQUEST_SENT && <Spinner /> }
                <Alert { ...statusMessage } closeAlert={ () => setStatusMessage(EMPTY_STATUS) } />
                <h5 style={{ marginTop: '2em' }}>Recover your password</h5>
                <p>Enter your email or username, then confirm Recaptcha to automatically get a recovery email.</p>

                <div className='input-field col s4'>
                    <i className='material-icons prefix'>account_circle</i>
                    <input id='emailOrUsername'
                           value={ identity.email || identity.username }
                           onChange={ e => updateIdentity(e.target.value, 'emailOrUsername') }
                           type='text'
                           className='validate'
                    />
                    <label htmlFor='emailOrUsername'>Email or Username</label>
                </div>

                <div className='col s12'>
                    <div className='col s12' style={{ marginTop: '1.5em' }}>
                        <ReCAPTCHA
                            sitekey='6LeXhN4UAAAAAHKW6-44VxtUVMYSlMPj04WRoC8z'
                            onChange={ val => updateIdentity(val as string, 'recaptcha') }
                        />
                    </div>
                </div>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ForgotPassword);