import React from 'react';
import {connect} from 'react-redux';

import {defaultTwoFa, ITwoFa, ITwoFactorDetail} from "../redux/interfaces";
import {invokeDisableTfaRequest, invokeEnableOrRenewTfaRequest} from "../redux/actions";
import ReCAPTCHA from "react-google-recaptcha";
import {checkSession, EMPTY_STATUS, EMPTY_STRING, IStatusMessage} from "../../../providers/helpers";
import * as accountConstants from '../redux/constants';
import {clearAuthUser} from "../../authentication/redux/actions";
import Alert from "../../../shared/Alert";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    enableRenewTfa: state.accountStore.enableRenewTfa,
    disableTfa: state.accountStore.disableTfa
});

const mapActionsToProps = {
    invokeEnableOrRenewTfaRequest,
    invokeDisableTfaRequest,
    clearAuthUser
};

const TwoFactorDetail = (props: ITwoFactorDetail) => {
    const [recaptchaToken, setRecaptchaToken] = React.useState(EMPTY_STRING);
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [shouldShowQrImage, setShouldShowQrImage] = React.useState(false);
    const [twoFa, setTwoFa] = React.useState(defaultTwoFa);

    React.useEffect(() => {
        setTwoFa(props.accountDetail.twoFa);
        setShouldShowQrImage(props.accountDetail.twoFaEnabled);
    }, []);

    React.useEffect(() => {
        if (recaptchaToken.length !== 0)
            props.invokeDisableTfaRequest(props.authUser, recaptchaToken);
    }, [recaptchaToken]);

    React.useEffect(() => {
        if (props.enableRenewTfa.action === accountConstants.ENABLE_RENEW_TFA_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.enableRenewTfa.error?.message);

        if (props.enableRenewTfa.action === accountConstants.ENABLE_RENEW_TFA_REQUEST_SUCCESS)
            if (props.enableRenewTfa.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.enableRenewTfa.payload.result === 0)
                setStatusMessage({ messages: props.enableRenewTfa.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setStatusMessage(EMPTY_STATUS);
                setShouldShowQrImage(true);
                setTwoFa(props.enableRenewTfa.payload.data as ITwoFa);
            }
    }, [props.enableRenewTfa]);

    React.useEffect(() => {
        if (props.disableTfa.action === accountConstants.DISABLE_TFA_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.disableTfa.error?.message);

        if (props.disableTfa.action === accountConstants.DISABLE_TFA_REQUEST_SUCCESS)
            if (props.disableTfa.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.disableTfa.payload.result === 0)
                setStatusMessage({ messages: props.disableTfa.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setStatusMessage({ messages: ['Two-Factor Authentication has been disabled.'], type: 'success' } as IStatusMessage);
                setShouldShowQrImage(false);
            }
    }, [props.disableTfa]);

    return (
        <div className='row'>
            <div className='col s12'>
                <b>Two Factor Authentication:</b><br />
                <Alert { ...statusMessage } />

                {
                    (
                        !shouldShowQrImage &&
                        <button className='btn waves-effect waves-light'
                                onClick={ () => props.invokeEnableOrRenewTfaRequest(props.authUser) }
                        >
                            <i className="fas fa-cog" />
                            &nbsp; Enable Two-Factor Authentication
                        </button>
                    ) ||
                    <div className='row'>
                        <div className='col s12'>
                            <p>To set up, please scan this QR image with Google Authenticator on&nbsp;
                                <a target='_blank' href='https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2'>Android</a> or&nbsp;
                                <a target='_blank' href='https://apps.apple.com/us/app/google-authenticator/id388497605'>iOS</a>.
                            </p>
                            <img className='responsive-img' src={ twoFa.qrImageUrl } alt='two-factor-auth-qr-image' width='300px' />
                            <p className='small-text'>QR Code: { twoFa.manualQrCode }</p>
                        </div>

                        <div className='col s12'>
                            <button className='btn waves-effect waves-light'
                                    onClick={ () => props.invokeEnableOrRenewTfaRequest(props.authUser) }
                            >
                                <i className="fas fa-redo" />
                                &nbsp; Renew
                            </button>

                            <a className='text-link red-text small-text' id='showRecaptcha2'
                               style={{ marginLeft: '1em' }}
                               onClick={ () => {
                                   $('#showRecaptcha2').hide();
                                   $('#recaptchaChecker2').show();
                               }}
                            >
                                <i className="fas fa-power-off" />
                                &nbsp; Disable
                            </a>
                        </div>

                        <div className='col s12' id='recaptchaChecker2' style={{ display: 'none' }}>
                            <p className='small-text'>Confirm the Recaptcha to disable Two-Factor Authentication:</p>

                            <ReCAPTCHA
                                sitekey='6LeXhN4UAAAAAHKW6-44VxtUVMYSlMPj04WRoC8z'
                                onChange={ val => setRecaptchaToken(val as string) }
                            />

                            <a className='text-link red-text small-text' onClick={ () => {
                                $('#recaptchaChecker2').hide();
                                $('#showRecaptcha2').show();
                            }}
                            >
                                Cancel
                            </a>
                        </div>
                    </div>
                }
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(TwoFactorDetail);