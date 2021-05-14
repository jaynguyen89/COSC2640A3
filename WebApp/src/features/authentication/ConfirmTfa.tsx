import React from 'react';
import { connect } from 'react-redux';
import _ from 'lodash';
import $ from 'jquery';

import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import {IAuthUser, IConfirmTfa} from "./redux/interfaces";
import {EMPTY_STATUS, EMPTY_STRING, IStatusMessage, setGlobalMessage} from "../../providers/helpers";
import {invokeConfirmTfaPinRequest, invokeSendPinToSmsAndEmailRequest, setAuthUser} from "./redux/actions";
import * as authenticationConstants from './redux/constants';

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    confirmTfa: state.authenticationStore.confirmTfa,
    sendPin: state.authenticationStore.sendPin
});

const mapActionsToProps = {
    invokeConfirmTfaPinRequest,
    setAuthUser,
    invokeSendPinToSmsAndEmailRequest
};

const ConfirmTfa = (props: IConfirmTfa) => {
    const [pin, setPin] = React.useState([-1, -1, -1, -1, -1, -1]);
    const [pinMessage, setPinMessage] = React.useState(EMPTY_STRING);
    const [sendPinMessage, setSendPinMessage] = React.useState(EMPTY_STRING);
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [shouldEnableDoneButton, setShouldEnableDoneButton] = React.useState(true);

    const handlePinEntry = (index: number, val: string) => {
        let clone = _.cloneDeep(pin);

        val = val.slice(val.length - 1);
        if (Number(val) < 0 || Number(val) > 9) {
            setPinMessage('Please input numbers from 0 to 9.');
            clone[index] = -1;
            setPin(clone);
            return;
        }

        setPinMessage(EMPTY_STRING);
        clone[index] = Number(val);

        setPin(clone);
        if (index < pin.length - 1) $(`#pin${ index + 2 }`).focus();
    }

    const attemptVerifyingPin = () => {
        if (!pin.every(item => item >= 0 && item <= 9)) {
            setPinMessage('Please input numbers from 0 to 9.');
            return;
        }

        setPinMessage(EMPTY_STRING);
        setShouldEnableDoneButton(false);

        const authUser = JSON.parse(localStorage.getItem('TempAuth') as string) as IAuthUser;
        props.invokeConfirmTfaPinRequest(authUser, pin.join(EMPTY_STRING));
    }

    React.useEffect(() => {
        setShouldEnableDoneButton(props.confirmTfa.action !== authenticationConstants.CONFIRM_TFA_REQUEST_SENT);

        if (props.confirmTfa.action === authenticationConstants.CONFIRM_TFA_REQUEST_FAILED)
            setStatusMessage({ messages: ['Unable to communicate with server: connection timed out.'], type: 'error' } as IStatusMessage);

        if (props.confirmTfa.action === authenticationConstants.CONFIRM_TFA_REQUEST_SUCCESS) {
            setStatusMessage(EMPTY_STATUS);

            if (props.confirmTfa.payload === null || props.confirmTfa.payload.result === 0) {
                setGlobalMessage({ messages: ['The confirmation has failed or hasn\'t gone through. Please try login again.'], type: 'success' } as IStatusMessage);
                window.location.href = '/';
            }

            if (props.confirmTfa.payload && props.confirmTfa.payload.result === 1) {
                const authUser = JSON.parse(localStorage.getItem('TempAuth') as string) as IAuthUser;
                localStorage.removeItem('TempAuth');
                props.setAuthUser(authUser);
                window.location.href = '/home';
            }
        }
    }, [props.confirmTfa]);

    if (props.authUser.isAuthenticated)
        window.location.href = '/home';

    React.useEffect(() => {
        if (props.sendPin.action === authenticationConstants.SEND_PIN_TO_SMS_EMAIL_REQUEST_FAILED)
            setSendPinMessage('Unable to communicate with server: connection timed out.');

        if (props.sendPin.action === authenticationConstants.SEND_PIN_TO_SMS_EMAIL_REQUEST_SUCCESS)
            if (props.sendPin.payload === null)
                setSendPinMessage('Failed to send your request to server. Please try again.');
            else if (props.sendPin.payload.result === 0)
                setSendPinMessage(props.sendPin.payload.messages[0]);
            else {
                setSendPinMessage(EMPTY_STRING);
                alert('Authentication PIN has been sent to you. Please check your SMS and email.');
            }
    }, [props.sendPin]);

    return (
        <div className='main-content'>
            <div className='row'>
                <div className='col s12'>
                    { props.confirmTfa.action === authenticationConstants.CONFIRM_TFA_REQUEST_SENT && <Spinner /> }
                    <Alert { ...statusMessage } closeAlert={ () => setStatusMessage(EMPTY_STATUS) } />

                    <h5 style={{ marginTop: '2em' }}>Enter Two-Factor Authentication PIN</h5>
                    <div className='error'>{ pinMessage }</div>

                    <div style={{ marginTop: '2em' }}>
                        <div className="col sm4">
                            <input id="pin1" type="number" max="9" min="0" step='1' className="pin-cell"
                                   value={ pin[0] < 0 ? EMPTY_STRING : pin[0] }
                                   onChange={ e => handlePinEntry(0, e.target.value) }
                            />
                        </div>
                        <div className="col sm4">
                            <input id="pin2" type="number" max="9" min="0" className="pin-cell"
                                   value={ pin[1] < 0 ? EMPTY_STRING : pin[1] }
                                   onChange={ e => handlePinEntry(1, e.target.value) }
                            />
                        </div>
                        <div className="col sm4">
                            <input id="pin3" type="number" max="9" min="0" className="pin-cell"
                                   value={ pin[2] < 0 ? EMPTY_STRING : pin[2] }
                                   onChange={ e => handlePinEntry(2, e.target.value) }
                            />
                        </div>
                        <div className="col sm4">
                            <input id="pin4" type="number" max="9" min="0" className="pin-cell"
                                   value={ pin[3] < 0 ? EMPTY_STRING : pin[3] }
                                   onChange={ e => handlePinEntry(3, e.target.value) }
                            />
                        </div>
                        <div className="col sm4">
                            <input id="pin5" type="number" max="9" min="0" className="pin-cell"
                                   value={ pin[4] < 0 ? EMPTY_STRING : pin[4] }
                                   onChange={ e => handlePinEntry(4, e.target.value) }
                            />
                        </div>
                        <div className="col sm4">
                            <input id="pin6" type="number" max="9" min="0" className="pin-cell"
                                   value={ pin[5] < 0 ? EMPTY_STRING : pin[5] }
                                   onChange={ e => handlePinEntry(5, e.target.value) }
                            />
                        </div>
                    </div>
                </div>

                <div className='col s12'>
                    <p>
                        Don't have access to your mobile authenticator app?<br/>
                        <a onClick={ () => props.invokeSendPinToSmsAndEmailRequest(JSON.parse(localStorage.getItem('TempAuth') as string) as IAuthUser) }
                           className='text-link'>Click here
                        </a> to get PIN by SMS and email.

                        <div className='error'>{ sendPinMessage }</div>

                        <div className='row'>
                            <div className='col s3' style={{ marginTop: '1.5em' }}>
                                { props.sendPin.action === authenticationConstants.SEND_PIN_TO_SMS_EMAIL_REQUEST_SENT && <Spinner /> }
                            </div>
                        </div>
                    </p>

                    <div className={ (shouldEnableDoneButton && 'btn waves-effect waves-light') || 'btn disabled' }
                         onClick={ attemptVerifyingPin }
                    >
                        Done
                    </div>
                </div>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ConfirmTfa);