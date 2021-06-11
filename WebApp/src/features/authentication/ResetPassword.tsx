import React from 'react';
import {connect} from "react-redux";
import {defaultPasswordReset, IPasswordReset, IResetPassword} from "./redux/interfaces";
import ReCAPTCHA from "react-google-recaptcha";
import {
    EMPTY_STATUS,
    EMPTY_STRING,
    IStatusMessage,
    removeGlobalMessage,
    setGlobalMessage
} from "../../providers/helpers";
import Alert from "../../shared/Alert";
import {invokePasswordResetRequest} from "./redux/actions";
import * as authenticationConstants from './redux/constants';
import Spinner from "../../shared/Spinner";

const mapStateToProps = (state: any) => ({
    resetPassword: state.authenticationStore.resetPassword
});

const mapActionsToProps = {
    invokePasswordResetRequest
};

const ResetPassword = (props: IResetPassword) => {
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [passwordReset, setPasswordReset] = React.useState(defaultPasswordReset);

    React.useEffect(() => {
        const storedGlobalMessage = sessionStorage.getItem('globalMessage');
        if (storedGlobalMessage) {
            setStatusMessage(JSON.parse(storedGlobalMessage) as IStatusMessage);
            removeGlobalMessage();
        }

        let accountId = localStorage.getItem('recoverPassword_accountId');
        if (accountId === null || accountId === undefined) {
            if (window.location.search.length === 0) {
                setStatusMessage({
                    messages: ['Unable to properly set up data for password recovery. Please try Forgot Password again.'],
                    type: 'error',
                    closeAlert: () => setStatusMessage(EMPTY_STATUS)
                } as IStatusMessage);
                return;
            }

            accountId = window.location.search.replace('?accountId=', EMPTY_STRING);
        }

        updatePasswordReset(accountId as string, 'accountId');
    }, []);

    const updatePasswordReset = (val: string, field: string) => {
        if (field === 'accountId') setPasswordReset({ ...passwordReset, accountId: val } as IPasswordReset);
        if (field === 'password') setPasswordReset({ ...passwordReset, password: val } as IPasswordReset);
        if (field === 'passwordConfirm') setPasswordReset({ ...passwordReset, passwordConfirm: val } as IPasswordReset);
        if (field === 'recoveryToken') setPasswordReset({ ...passwordReset, recoveryToken: val } as IPasswordReset);
        if (field === 'recaptcha') setPasswordReset({ ...passwordReset, recaptchaToken: val } as IPasswordReset);
    }

    React.useEffect(() => {
        if (passwordReset.accountId.length !== 0 &&
            passwordReset.password.length !== 0 &&
            passwordReset.passwordConfirm.length !== 0 &&
            passwordReset.recoveryToken.length !== 0 &&
            passwordReset.recaptchaToken.length !== 0
        ) props.invokePasswordResetRequest(passwordReset);
    }, [passwordReset]);

    React.useEffect(() => {
        if (props.resetPassword.action === authenticationConstants.RESET_PASSWORD_REQUEST_FAILED)
            setStatusMessage({ messages: ['Unable to communicate with server: connection timed out.'], type: 'error' } as IStatusMessage);

        if (props.resetPassword.action === authenticationConstants.RESET_PASSWORD_REQUEST_SUCCESS)
            if (props.resetPassword.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.resetPassword.payload.result === 0)
                setStatusMessage({ messages: props.resetPassword.payload.messages, type: 'error' } as IStatusMessage);
            else {
                localStorage.removeItem('recoverPassword_accountId');
                setGlobalMessage({ messages: ['Your new password is updated successfully. You can now sign in.'], type: 'success' } as IStatusMessage);
                window.location.href = '/';
            }
    }, [props.resetPassword]);

    return (
        <div className='main-content'>
            <div className='row'>
                <Alert { ...statusMessage } closeAlert={ () => setStatusMessage(EMPTY_STATUS) } />

                <h5 style={{ marginTop: '2em' }}>Reset your password</h5>
                <p>Enter your new password, password confirm and click the Recaptcha checkbox.</p>

                { props.resetPassword.action === authenticationConstants.RESET_PASSWORD_REQUEST_SENT && <Spinner /> }

                <div className='row'>
                    <div className='input-field col m5 s12'>
                        <i className='material-icons prefix'>lock</i>
                        <input id='password'
                               value={ passwordReset.password }
                               onChange={ e => updatePasswordReset(e.target.value, 'password') }
                               type='password'
                               className='validate'
                        />
                        <label htmlFor='password'>New Password</label>
                    </div>
                </div>

                <div className='row'>
                    <div className='input-field col m5 s12'>
                        <i className='material-icons prefix'>lock</i>
                        <input id='passwordConfirm'
                               value={ passwordReset.passwordConfirm }
                               onChange={ e => updatePasswordReset(e.target.value, 'passwordConfirm') }
                               type='password'
                               className='validate'
                        />
                        <label htmlFor='passwordConfirm'>Confirm Password</label>
                    </div>
                </div>

                <div className='row'>
                    <div className='input-field col m5 s12'>
                        <i className='material-icons prefix'>pin</i>
                        <input id='recoveryToken'
                               value={ passwordReset.recoveryToken }
                               onChange={ e => updatePasswordReset(e.target.value, 'recoveryToken') }
                               type='text'
                               className='validate'
                        />
                        <label htmlFor='recoveryToken'>Recovery Token</label>
                    </div>
                </div>

                <div className='col s12'>
                    <div className='col s12' style={{ marginTop: '1.5em' }}>
                        <ReCAPTCHA
                            sitekey='6LeXhN4UAAAAAHKW6-44VxtUVMYSlMPj04WRoC8z'
                            onChange={ val => updatePasswordReset(val as string, 'recaptcha') }
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
)(ResetPassword);