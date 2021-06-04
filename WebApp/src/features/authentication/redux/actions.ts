import * as authenticationConstants from './constants';
import * as authenticationServices from './services';
import {IAccountData, IActivationData, IAuthUser, ICredentials, IIdentity, IPasswordReset} from "./interfaces";

export const invokeAuthenticationRequest = (credentials: ICredentials) => {
    return (dispatch: any) => {
        dispatch({ type: authenticationConstants.AUTHENTICATION_BEGIN });

        authenticationServices.sendLoginRequest(credentials)
            .then(response => dispatch({
                type: authenticationConstants.AUTHENTICATION_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: authenticationConstants.AUTHENTICATION_FAILED,
                error
            }))
    }
}

export const setAuthUser = (authUser: IAuthUser) => {
    return (dispatch: any) => dispatch({
        type: authenticationConstants.AUTHENTICATED,
        payload: authUser
    });
}

export const loadAuthUser = () => {
    const authUser = authenticationServices.loadAuthUserFromCookies();

    return (dispatch: any) => dispatch({
        type: authenticationConstants.LOAD_AUTH_DATA,
        payload: authUser
    });
}

export const clearAuthUser = () => {
    authenticationServices.clearAuthUserInCookie();
    return (dispatch: any) => dispatch({ type: authenticationConstants.NO_AUTHENTICATION });
}

export const invokeRegistrationRequest = (accountData: IAccountData) => {
    return (dispatch: any) => {
        dispatch({ type: authenticationConstants.REGISTRATION_REQUEST_SENT });

        authenticationServices.sendRegistrationRequest(accountData)
            .then(response => dispatch({
                type: authenticationConstants.REGISTRATION_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: authenticationConstants.REGISTRATION_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeSignOutRequest = (auth: IAuthUser) => {
    return (dispatch: any) => {
        dispatch({ type: authenticationConstants.SIGNOUT_REQUEST_SENT });

        authenticationServices.sendSignOutRequest(auth)
            .then(response => dispatch({
                type: authenticationConstants.SIGNOUT_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: authenticationConstants.SIGNOUT_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeConfirmTfaPinRequest = (auth: IAuthUser, pin: string) => {
    return (dispatch: any) => {
        dispatch({ type: authenticationConstants.CONFIRM_TFA_REQUEST_SENT });

        authenticationServices.sendConfirmTfaPinRequest(auth, pin)
            .then(response => dispatch({
                type: authenticationConstants.CONFIRM_TFA_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: authenticationConstants.CONFIRM_TFA_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeAccountActivationRequest = (activationData: IActivationData) => {
    return (dispatch: any) => {
        dispatch({ type: authenticationConstants.ACCOUNT_ACTIVATION_REQUEST_SENT });

        authenticationServices.sendAccountActivationRequest(activationData)
            .then(response => dispatch({
                type: authenticationConstants.ACCOUNT_ACTIVATION_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: authenticationConstants.ACCOUNT_ACTIVATION_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeSendPinToSmsAndEmailRequest = (auth: IAuthUser) => {
    return (dispatch: any) => {
        dispatch({ type: authenticationConstants.SEND_PIN_TO_SMS_EMAIL_REQUEST_SENT });

        authenticationServices.sendPinToSmsAndEmailRequest(auth)
            .then(response => dispatch({
                type: authenticationConstants.SEND_PIN_TO_SMS_EMAIL_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: authenticationConstants.SEND_PIN_TO_SMS_EMAIL_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeForgotPasswordRequest = (identity: IIdentity) => {
    return (dispatch: any) => {
        dispatch({ type: authenticationConstants.FORGOT_PASSWORD_REQUEST_SENT });

        authenticationServices.sendForgotPasswordRequest(identity)
            .then(response => dispatch({
                type: authenticationConstants.FORGOT_PASSWORD_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: authenticationConstants.FORGOT_PASSWORD_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeSwitchRoleRequest = (auth: IAuthUser) => {
    return (dispatch: any) => {
        dispatch({ type: authenticationConstants.SWITCH_ROLE_REQUEST_SENT });

        authenticationServices.sendSwitchRoleRequest(auth)
            .then(response => dispatch({
                type: authenticationConstants.SWITCH_ROLE_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: authenticationConstants.SWITCH_ROLE_REQUEST_FAILED,
                error
            }))
    }
}

export const invokePasswordResetRequest = (data: IPasswordReset) => {
    return (dispatch: any) => {
        dispatch({ type: authenticationConstants.RESET_PASSWORD_REQUEST_SENT });

        authenticationServices.sendPasswordResetRequest(data)
            .then(response => dispatch({
                type: authenticationConstants.RESET_PASSWORD_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: authenticationConstants.RESET_PASSWORD_REQUEST_FAILED,
                error
            }))
    }
}