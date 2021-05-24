import React from 'react';
import {connect} from 'react-redux';

import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import * as authenticationConstants from "./redux/constants";
import {
    EMPTY_STATUS,
    EMPTY_STRING,
    IStatusMessage,
    removeGlobalMessage,
    setGlobalMessage
} from "../../providers/helpers";
import _ from "lodash";
import $ from "jquery";
import {defaultActivationData, IActivateAccount} from "./redux/interfaces";
import ReCAPTCHA from "react-google-recaptcha";
import {invokeAccountActivationRequest} from "./redux/actions";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    activateAccount: state.authenticationStore.activateAccount
});

const mapActionsToProps = {
    invokeAccountActivationRequest
};

const ActivateAccount = (props: IActivateAccount) => {
    const [code, setCode] = React.useState([-1, -1, -1, -1, -1, -1]);
    const [activationData, setActivationData] = React.useState(defaultActivationData);
    const [message, setMessage] = React.useState(EMPTY_STRING);
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);

    const handleCodeEntry = (index: number, val: string) => {
        let clone = _.cloneDeep(code);

        val = val.slice(val.length - 1);
        if (Number(val) < 0 || Number(val) > 9) {
            setMessage('Please input numbers from 0 to 9.');
            clone[index] = -1;
            setCode(clone);
            return;
        }

        setMessage(EMPTY_STRING);
        clone[index] = Number(val);

        setCode(clone);
        if (index < code.length - 1) $(`#pin${ index + 2 }`).focus();
    }

    const updateActivationData = (value: string, field: string) => {
        if (field === 'emailOrUsername') {
            if ((value as string).indexOf('@') !== -1) setActivationData({ ...activationData, email: value as string, username: EMPTY_STRING });
            else setActivationData({ ...activationData, username: value as string, email: EMPTY_STRING });
        }

        if (field === 'recaptcha') setActivationData({ ...activationData, recaptchaToken: value });
    }

    React.useEffect(() => {
        const storedGlobalMessage = sessionStorage.getItem('globalMessage');
        if (storedGlobalMessage) {
            setStatusMessage(JSON.parse(storedGlobalMessage) as IStatusMessage);
            removeGlobalMessage();
        }
    }, []);

    React.useEffect(() => {
        const shouldActivate =
            code.every(item => item >= 0 && item <= 9) &&
            (activationData.email.length > 0 || activationData.username.length > 0) &&
            activationData.recaptchaToken.length > 0;

        if (shouldActivate) {
            setMessage(EMPTY_STRING);
            let clone = _.cloneDeep(activationData);

            clone.confirmCode = code.join(EMPTY_STRING);
            props.invokeAccountActivationRequest(clone);
        }
    }, [code, activationData]);

    if (props.authUser.isAuthenticated)
        window.location.href = '/home';

    React.useEffect(() => {
        if (props.activateAccount.action === authenticationConstants.ACCOUNT_ACTIVATION_REQUEST_FAILED)
            setStatusMessage({ messages: ['Unable to communicate with server: connection timed out.'], type: 'error' } as IStatusMessage);

        if (props.activateAccount.action === authenticationConstants.ACCOUNT_ACTIVATION_REQUEST_SUCCESS)
            if (props.activateAccount.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.activateAccount.payload.result === 0)
                setStatusMessage({ messages: props.activateAccount.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setGlobalMessage({ messages: ['Your account has been successfully activated. You can now login.'], type: 'success' } as IStatusMessage);
                window.location.href = '/';
            }
    }, [props.activateAccount]);

    return (
        <div className='main-content'>
            <div className='row'>
                { props.activateAccount.action === authenticationConstants.ACCOUNT_ACTIVATION_REQUEST_SENT && <Spinner /> }
                <Alert { ...statusMessage } closeAlert={ () => setStatusMessage(EMPTY_STATUS) } />
                <h5 style={{ marginTop: '2em' }}>Activate Account</h5>

                <div className='input-field col s4'>
                    <i className='material-icons prefix'>account_circle</i>
                    <input id='emailOrUsername'
                           value={ activationData.email || activationData.username }
                           onChange={ e => updateActivationData(e.target.value, 'emailOrUsername') }
                           type='text'
                           className='validate'
                    />
                    <label htmlFor='emailOrUsername'>Email or Username</label>
                </div>

                <div className='col s12'>
                    <h6>Enter activation code</h6>
                    <div className='error'>{ message }</div>

                    <div style={{ marginTop: '2em' }}>
                        <div className="col sm4">
                            <input id="pin1" type="number" max="9" min="0" step='1' className="pin-cell"
                                   value={ code[0] < 0 ? EMPTY_STRING : code[0] }
                                   onChange={ e => handleCodeEntry(0, e.target.value) }
                            />
                        </div>
                        <div className="col sm4">
                            <input id="pin2" type="number" max="9" min="0" className="pin-cell"
                                   value={ code[1] < 0 ? EMPTY_STRING : code[1] }
                                   onChange={ e => handleCodeEntry(1, e.target.value) }
                            />
                        </div>
                        <div className="col sm4">
                            <input id="pin3" type="number" max="9" min="0" className="pin-cell"
                                   value={ code[2] < 0 ? EMPTY_STRING : code[2] }
                                   onChange={ e => handleCodeEntry(2, e.target.value) }
                            />
                        </div>
                        <div className="col sm4">
                            <input id="pin4" type="number" max="9" min="0" className="pin-cell"
                                   value={ code[3] < 0 ? EMPTY_STRING : code[3] }
                                   onChange={ e => handleCodeEntry(3, e.target.value) }
                            />
                        </div>
                        <div className="col sm4">
                            <input id="pin5" type="number" max="9" min="0" className="pin-cell"
                                   value={ code[4] < 0 ? EMPTY_STRING : code[4] }
                                   onChange={ e => handleCodeEntry(4, e.target.value) }
                            />
                        </div>
                        <div className="col sm4">
                            <input id="pin6" type="number" max="9" min="0" className="pin-cell"
                                   value={ code[5] < 0 ? EMPTY_STRING : code[5] }
                                   onChange={ e => handleCodeEntry(5, e.target.value) }
                            />
                        </div>
                    </div>

                    <div className='col s12' style={{ marginTop: '1.5em' }}>
                        <ReCAPTCHA
                            sitekey='6LeXhN4UAAAAAHKW6-44VxtUVMYSlMPj04WRoC8z'
                            onChange={ val => updateActivationData(val as string, 'recaptcha') }
                        />
                    </div>

                    <div className='col s12 small-text' style={{ marginTop: '1.5em' }}>
                        If your activation code has expired. Please <a className='text-link' href='/forgot-password'>click here</a> to request another activation code.
                    </div>
                </div>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ActivateAccount);