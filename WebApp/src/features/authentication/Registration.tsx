import React from 'react';
import { connect } from "react-redux";

import Alert from "../../shared/Alert";
import Spinner from "../../shared/Spinner";

import * as authenticationConstants from './redux/constants';
import { EMPTY_ACCOUNT, IRegistrationComponent } from "./redux/interfaces";
import { EMPTY_STATUS, IStatusMessage, setGlobalMessage } from "../../providers/helpers";
import { invokeRegistrationRequest } from './redux/actions';
import ReCAPTCHA from "react-google-recaptcha";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    registration: state.authenticationStore.register
});

const mapActionsToProps = {
    invokeRegistrationRequest
};

const Registration = (props: IRegistrationComponent) => {
    const [accountData, setAccountData] = React.useState(EMPTY_ACCOUNT);
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);

    const [shouldEnableDoneButton, setShouldEnableDoneButton] = React.useState(true);

    const updateAccountData = (value: string, field: string) => {
        if (field === 'email') setAccountData({ ...accountData, email: value });
        if (field === 'username') setAccountData({ ...accountData, username: value });
        if (field === 'password') setAccountData({ ...accountData, password: value });
        if (field === 'passwordConfirm') setAccountData({ ...accountData, passwordConfirm: value });
        if (field === 'phoneNumber') setAccountData({ ...accountData, phoneNumber: value });
        if (field === 'preferredName') setAccountData({ ...accountData, preferredName: value });
        if (field === 'recaptcha') setAccountData({ ...accountData, recaptchaToken: value });
    }

    const attemptRegistration = () => props.invokeRegistrationRequest(accountData);

    React.useEffect(() => {
        if (props.registration.action === authenticationConstants.REGISTRATION_REQUEST_SENT)
            setShouldEnableDoneButton(false);

        if (props.registration.action === authenticationConstants.REGISTRATION_REQUEST_FAILED) {
            setStatusMessage({ messages: ['Unable to communicate with server: connection timed out.'], type: 'error' } as IStatusMessage);
            setShouldEnableDoneButton(true);
        }

        if (props.registration.action === authenticationConstants.REGISTRATION_REQUEST_SUCCESS)
            if (props.registration.payload === null) {
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
                setShouldEnableDoneButton(true);
            }
            else if (props.registration.payload.result === 0) {
                setStatusMessage({ messages: props.registration.payload.data, type: 'error' } as IStatusMessage);
                setShouldEnableDoneButton(true);
            }
            else {
                setStatusMessage(EMPTY_STATUS);
                setGlobalMessage({ messages: ['Your account has been created. Please confirm to activate your account before you can login.'], type: 'success' } as IStatusMessage);
                window.location.href = '/activate';
            }
    }, [props.registration]);

    if (props.authUser.isAuthenticated)
        window.location.href = '/home';

    return (
        <div className='container'>
            <div className='row'>
                <div className='login-area'>
                    <Alert { ...statusMessage } />
                    { props.registration.action === authenticationConstants.REGISTRATION_REQUEST_SENT && <Spinner /> }

                    <div className='card horizontal'>
                        <div className='card-content'>
                            <span className='card-title'>
                                <i className='material-icons teal-text'>playlist_add</i>
                                Register an account
                            </span>

                            <div className='row'>
                                <div className='input-field col s12'>
                                    <i className='material-icons prefix'>alternate_email</i>
                                    <input id='email'
                                           value={ accountData.email }
                                           onChange={ e => updateAccountData(e.target.value, 'email') }
                                           type='text'
                                           className='validate'
                                    />
                                    <label htmlFor='email'>Email</label>
                                </div>

                                <div className='input-field col s12'>
                                    <i className='material-icons prefix'>account_circle</i>
                                    <input id='username'
                                           value={ accountData.username }
                                           onChange={ e => updateAccountData(e.target.value, 'username') }
                                           type='text'
                                           className='validate'
                                    />
                                    <label htmlFor='username'>Username</label>
                                </div>

                                <div className='input-field col m6 s12'>
                                    <i className='material-icons prefix'>lock</i>
                                    <input id='password'
                                           value={ accountData.password }
                                           onChange={ e => updateAccountData(e.target.value, 'password') }
                                           type='password'
                                           className='validate'
                                    />
                                    <label htmlFor='password'>Password</label>
                                </div>

                                <div className='input-field col m6 s12'>
                                    <i className='material-icons prefix'>lock</i>
                                    <input id='passwordConfirm'
                                           value={ accountData.passwordConfirm }
                                           onChange={ e => updateAccountData(e.target.value, 'passwordConfirm') }
                                           type='password'
                                           className='validate'
                                    />
                                    <label htmlFor='passwordConfirm'>Confirm Password</label>
                                </div>

                                <div className='input-field col m6 s12'>
                                    <i className='material-icons prefix'>phone</i>
                                    <input id='phoneNumber'
                                           value={ accountData.phoneNumber }
                                           onChange={ e => updateAccountData(e.target.value, 'phoneNumber') }
                                           type='text'
                                           className='validate'
                                    />
                                    <label htmlFor='phoneNumber'>Phone Number</label>
                                </div>

                                <div className='input-field col m6 s12'>
                                    <i className='material-icons prefix'>history_edu</i>
                                    <input id='preferredName'
                                           value={ accountData.preferredName }
                                           onChange={ e => updateAccountData(e.target.value, 'preferredName') }
                                           type='text'
                                           className='validate'
                                    />
                                    <label htmlFor='preferredName'>Full Name</label>
                                </div>

                                <div className='col s12' style={{ marginBottom: '1.5em' }}>
                                    <ReCAPTCHA
                                        sitekey='6LeXhN4UAAAAAHKW6-44VxtUVMYSlMPj04WRoC8z'
                                        onChange={ val => updateAccountData(val as string, 'recaptcha') }
                                    />
                                </div>

                                <div className='col s12'>
                                    <button className={ shouldEnableDoneButton ? 'btn waves-effect waves-light' : 'btn disabled' }
                                            onClick={ attemptRegistration }
                                    >
                                        <i className='material-icons right'>done</i>
                                        Done
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className='row'>
                        <div className='col s12 center-align'>
                            Already have an account? <a href='/'>Login now</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(Registration);