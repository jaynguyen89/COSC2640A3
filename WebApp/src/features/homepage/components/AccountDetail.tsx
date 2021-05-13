import React from 'react';
import {connect} from 'react-redux';
import {IAccountDetail, ITeacherDetail} from "../redux/interfaces";
import {
    invokeAddPhoneNumberRequest,
    invokeConfirmPhoneNumberRequest,
    invokeNewSmsTokenRequest,
    invokeRemovePhoneNumberRequest
} from "../redux/actions";
import {checkSession, EMPTY_STATUS, EMPTY_STRING, IStatusMessage, modalOptions} from "../../../providers/helpers";
import M from 'materialize-css';
import ReCAPTCHA from "react-google-recaptcha";
import Spinner from "../../../shared/Spinner";
import * as accountConstants from '../redux/constants';
import Alert from "../../../shared/Alert";
import {clearAuthUser} from "../../authentication/redux/actions";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    confirmPhoneNumber: state.accountStore.confirmPhoneNumber,
    newSmsToken: state.accountStore.newSmsToken,
    removePhoneNumber: state.accountStore.removePhoneNumber,
    addPhoneNumber: state.accountStore.addPhoneNumber
});

const mapActionsToProps = {
    invokeConfirmPhoneNumberRequest,
    invokeNewSmsTokenRequest,
    invokeRemovePhoneNumberRequest,
    invokeAddPhoneNumberRequest,
    clearAuthUser
};

const AccountDetail = (props: IAccountDetail) => {
    const [phoneNumber, setPhoneNumber] = React.useState(EMPTY_STRING);
    const [smsToken, setSmsToken] = React.useState(EMPTY_STRING);
    const [recaptchaToken, setRecaptchaToken] = React.useState(EMPTY_STRING);
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [shouldConfirmPhoneNumber, setShouldConfirmPhoneNumber] = React.useState(false);

    React.useEffect(() => {
        setPhoneNumber(props.accountDetail.phoneNumber);
        setShouldConfirmPhoneNumber(!props.accountDetail.phoneNumberConfirmed);
    }, [props.accountDetail.phoneNumber]);

    React.useEffect(() => {
        if (recaptchaToken.length !== 0)
            props.invokeNewSmsTokenRequest(props.authUser, recaptchaToken);
    }, [recaptchaToken]);

    React.useEffect(() => {
        if (props.confirmPhoneNumber.action === accountConstants.CONFIRM_PHONE_NUMBER_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.confirmPhoneNumber.error?.message);

        if (props.confirmPhoneNumber.action === accountConstants.CONFIRM_PHONE_NUMBER_REQUEST_SUCCESS)
            if (props.confirmPhoneNumber.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.confirmPhoneNumber.payload.result === 0)
                setStatusMessage({ messages: props.confirmPhoneNumber.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setStatusMessage({ messages: ['Your phone number has been confirmed.'], type: 'success' } as IStatusMessage);
                setShouldConfirmPhoneNumber(false);
            }
    }, [props.confirmPhoneNumber]);

    React.useEffect(() => {
        if (props.newSmsToken.action === accountConstants.NEW_SMS_TOKEN_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.newSmsToken.error?.message);

        if (props.newSmsToken.action === accountConstants.NEW_SMS_TOKEN_REQUEST_SUCCESS)
            if (props.newSmsToken.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.newSmsToken.payload.result === 0)
                setStatusMessage({ messages: props.newSmsToken.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setStatusMessage({ messages: ['New SMS token has been sent to your phone.'], type: 'success' } as IStatusMessage);
                $('#recaptchaChecker1').hide();
                $('#showRecaptcha1').show();
            }
    }, [props.newSmsToken]);

    React.useEffect(() => {
        if (props.removePhoneNumber.action === accountConstants.REMOVE_PHONE_NUMBER_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.removePhoneNumber.error?.message);

        if (props.removePhoneNumber.action === accountConstants.REMOVE_PHONE_NUMBER_REQUEST_SUCCESS)
            if (props.removePhoneNumber.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.removePhoneNumber.payload.result === 0)
                setStatusMessage({ messages: props.removePhoneNumber.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setStatusMessage({ messages: ['Your phone number has been removed.'], type: 'success' } as IStatusMessage);
                setPhoneNumber(EMPTY_STRING);
            }
    }, [props.removePhoneNumber]);

    React.useEffect(() => {
        if (props.addPhoneNumber.action === accountConstants.ADD_PHONE_NUMBER_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.addPhoneNumber.error?.message);

        if (props.addPhoneNumber.action === accountConstants.ADD_PHONE_NUMBER_REQUEST_SUCCESS)
            if (props.addPhoneNumber.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.addPhoneNumber.payload.result === 0)
                setStatusMessage({ messages: props.addPhoneNumber.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setStatusMessage({messages: ['Your phone number has been added.'], type: 'success'} as IStatusMessage);
                setShouldConfirmPhoneNumber(true);
            }
    }, [props.addPhoneNumber]);

    return (
        <div className='row'>
            <div className='col s4'><b>Email:</b> { props.accountDetail.email }</div>
            <div className='col s4'><b>Username:</b> { props.accountDetail.username }</div>
            <div className='col s4'><b>Full Name:</b> { props.accountDetail.preferredName }</div>
            <div className='col s12' style={{ marginTop: '1em' }}>
                {
                    (
                        props.confirmPhoneNumber.action === accountConstants.CONFIRM_PHONE_NUMBER_REQUEST_SENT ||
                        props.newSmsToken.action === accountConstants.NEW_SMS_TOKEN_REQUEST_SENT ||
                        props.removePhoneNumber.action === accountConstants.REMOVE_PHONE_NUMBER_REQUEST_SENT
                    ) &&
                    <div className='row'><Spinner /></div>
                }
                <Alert { ...statusMessage } />

                <div className='input-field col s4'>
                    <i className='material-icons prefix'>phone</i>
                    <input id='phoneNumber'
                           type='text'
                           className='validate'
                           value={ phoneNumber }
                           onChange={ e => setPhoneNumber(e.target.value) }
                    />
                    <label htmlFor='phoneNumber'>Phone Number</label>
                </div>

                <div className='input-field col s8'>
                    {
                        (
                            props.accountDetail.phoneNumber &&
                            <button className='btn waves-effect waves-light amber'
                                    onClick={ () => props.invokeRemovePhoneNumberRequest(props.authUser) }
                            >
                                <i className="fas fa-trash-alt" />
                                &nbsp; Remove
                            </button>
                        ) ||
                        <button className='btn waves-effect waves-light'
                                onClick={ () => props.invokeAddPhoneNumberRequest(props.authUser, phoneNumber) }
                        >
                            <i className="fas fa-plus-circle" />
                            &nbsp; Add
                        </button>
                    }

                    {
                        shouldConfirmPhoneNumber &&
                        <>
                            <button className='btn waves-effect waves-light modal-trigger'
                                    style={{ marginLeft: '1em' }}
                                    data-target='confirmPhoneNumberModal'
                                    onClick={ () => {
                                        const confirmPhoneModal = document.querySelectorAll('#confirmPhoneNumberModal');
                                        M.Modal.init(confirmPhoneModal, modalOptions);
                                    }}
                            >
                                Confirm
                            </button>

                            <p className='small-text' id='showRecaptcha1'>
                                If your SMS token has expired.&nbsp;
                                <a className='text-link'
                                   onClick={ () => {
                                       $('#showRecaptcha1').hide();
                                       $('#recaptchaChecker1').show();
                                   }}
                                >
                                    Click here
                                </a> to request a new SMS token.
                            </p>

                            <div className='col s12' id='recaptchaChecker1' style={{ display: 'none' }}>
                                <p className='small-text'>Confirm the Recaptcha to get a new SMS token:</p>

                                <ReCAPTCHA
                                    sitekey='6LeXhN4UAAAAAHKW6-44VxtUVMYSlMPj04WRoC8z'
                                    onChange={ val => setRecaptchaToken(val as string) }
                                />

                                <a className='text-link red-text small-text' onClick={ () => {
                                    $('#recaptchaChecker1').hide();
                                    $('#showRecaptcha1').show();
                                }}
                                >
                                    Cancel
                                </a>
                            </div>
                        </>
                    }
                </div>
            </div>

            <div className='modal' id='confirmPhoneNumberModal'>
                <div className='modal-content'>
                    <h5 className='card-title'>Enter your SMS token to confirm</h5>
                    <div className='input-field col s8'>
                        <i className='material-icons prefix'>keyboard</i>
                        <input id='smsToken'
                               type='text'
                               className='validate'
                               value={ smsToken }
                               onChange={ e => setSmsToken(e.target.value) }
                        />
                        <label htmlFor='smsToken'>SMS Token</label>
                    </div>

                    <div className='input-field col s4 center-align'>
                        <button className='btn waves-effect waves-light'
                                style={{ marginLeft: '1em' }}
                                onClick={ () => {
                                    props.invokeConfirmPhoneNumberRequest(props.authUser, smsToken);
                                    const confirmPhoneModal = document.querySelectorAll('#confirmPhoneNumberModal');
                                    M.Modal.init(confirmPhoneModal, {});
                                }}
                        >
                            <i className="fas fa-check" />
                            &nbsp; Confirm
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(AccountDetail);