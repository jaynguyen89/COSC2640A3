import * as authenticationConstants from './constants';
import produce from 'immer';
import {DEFAULT_ACTION_RESULT, EMPTY_STRING, IActionResult} from "../../../providers/helpers";
import {DEFAULT_AUTH_USER, IActivateAccount, IAuthUser} from "./interfaces";

interface IAuthenticationStore {
    authenticate: IActionResult,
    register: IActionResult,
    authUser: IAuthUser,
    unauthenticate: IActionResult
    confirmTfa : IActionResult,
    activateAccount: IActionResult,
    sendPin: IActionResult,
    forgotPassword: IActionResult
}

const initialState: IAuthenticationStore = {
    authenticate: DEFAULT_ACTION_RESULT,
    register: DEFAULT_ACTION_RESULT,
    authUser: DEFAULT_AUTH_USER,
    unauthenticate: DEFAULT_ACTION_RESULT,
    confirmTfa: DEFAULT_ACTION_RESULT,
    activateAccount: DEFAULT_ACTION_RESULT,
    sendPin: DEFAULT_ACTION_RESULT,
    forgotPassword: DEFAULT_ACTION_RESULT
};

const reducer = produce((state, action) => {
    switch (action.type) {
        case authenticationConstants.AUTHENTICATION_BEGIN:
            state.authenticate.action = authenticationConstants.AUTHENTICATION_BEGIN;
            state.authenticate.payload = null;
            state.authenticate.error = null;
            return;
        case authenticationConstants.AUTHENTICATION_SUCCESS:
            state.authenticate.action = authenticationConstants.AUTHENTICATION_SUCCESS;
            state.authenticate.payload = action.payload;
            state.authenticate.error = null;
            return;
        case authenticationConstants.AUTHENTICATION_FAILED:
            state.authenticate.action = authenticationConstants.AUTHENTICATION_FAILED;
            state.authenticate.error = action.error;
            state.authenticate.payload = null;
            return;
        case authenticationConstants.AUTHENTICATED:
            state.authenticate.action = EMPTY_STRING;
            state.authenticate.payload = null;
            state.authenticate.error = null;

            state.authUser.isAuthenticated = true;
            state.authUser.authToken = action.payload.authToken;
            state.authUser.accountId = action.payload.accountId;
            state.authUser.role = action.payload.role;
            return;
        case authenticationConstants.LOAD_AUTH_DATA:
            state.authUser.isAuthenticated = action.payload.isAuthenticated;
            state.authUser.authToken = action.payload.authToken;
            state.authUser.accountId = action.payload.accountId;
            state.authUser.role = action.payload.role;
            return;
        case authenticationConstants.REGISTRATION_REQUEST_SENT:
            state.register.action = authenticationConstants.REGISTRATION_REQUEST_SENT;
            state.register.payload = null;
            state.register.error = null;
            return;
        case authenticationConstants.REGISTRATION_REQUEST_SUCCESS:
            state.register.action = authenticationConstants.REGISTRATION_REQUEST_SUCCESS;
            state.register.payload = action.payload;
            state.register.error = null;
            return;
        case authenticationConstants.REGISTRATION_REQUEST_FAILED:
            state.register.action = authenticationConstants.REGISTRATION_REQUEST_FAILED;
            state.register.payload = null;
            state.register.error = action.error;
            return;
        case authenticationConstants.SIGNOUT_REQUEST_SENT:
            state.unauthenticate.action = authenticationConstants.SIGNOUT_REQUEST_SENT;
            state.unauthenticate.payload = null;
            state.unauthenticate.error = null;
            return;
        case authenticationConstants.SIGNOUT_REQUEST_SUCCESS:
            state.unauthenticate.action = authenticationConstants.SIGNOUT_REQUEST_SUCCESS;
            state.unauthenticate.payload = action.payload;
            state.unauthenticate.error = null;
            return;
        case authenticationConstants.SIGNOUT_REQUEST_FAILED:
            state.unauthenticate.action = authenticationConstants.SIGNOUT_REQUEST_FAILED;
            state.unauthenticate.payload = null;
            state.unauthenticate.error = action.error;
            return;
        case authenticationConstants.NO_AUTHENTICATION:
            state.authUser = DEFAULT_AUTH_USER;
            return;
        case authenticationConstants.CONFIRM_TFA_REQUEST_SENT:
            state.confirmTfa.action = authenticationConstants.CONFIRM_TFA_REQUEST_SENT;
            state.confirmTfa.payload = null;
            state.confirmTfa.error = null;
            return;
        case authenticationConstants.CONFIRM_TFA_REQUEST_SUCCESS:
            state.confirmTfa.action = authenticationConstants.CONFIRM_TFA_REQUEST_SUCCESS;
            state.confirmTfa.payload = action.payload;
            state.confirmTfa.error = null;
            return;
        case authenticationConstants.CONFIRM_TFA_REQUEST_FAILED:
            state.confirmTfa.action = authenticationConstants.CONFIRM_TFA_REQUEST_FAILED;
            state.confirmTfa.payload = null;
            state.confirmTfa.error = action.error;
            return;
        case authenticationConstants.ACCOUNT_ACTIVATION_REQUEST_SENT:
            state.activateAccount.action = authenticationConstants.ACCOUNT_ACTIVATION_REQUEST_SENT;
            state.activateAccount.payload = null;
            state.activateAccount.error = null;
            return;
        case authenticationConstants.ACCOUNT_ACTIVATION_REQUEST_SUCCESS:
            state.activateAccount.action = authenticationConstants.ACCOUNT_ACTIVATION_REQUEST_SUCCESS;
            state.activateAccount.payload = action.payload;
            state.activateAccount.error = null;
            return;
        case authenticationConstants.ACCOUNT_ACTIVATION_REQUEST_FAILED:
            state.activateAccount.action = authenticationConstants.ACCOUNT_ACTIVATION_REQUEST_FAILED;
            state.activateAccount.payload = null;
            state.activateAccount.error = action.error;
            return;
        case authenticationConstants.SEND_PIN_TO_SMS_EMAIL_REQUEST_SENT:
            state.sendPin.action = authenticationConstants.SEND_PIN_TO_SMS_EMAIL_REQUEST_SENT;
            state.sendPin.payload = null;
            state.sendPin.error = null;
            return;
        case authenticationConstants.SEND_PIN_TO_SMS_EMAIL_REQUEST_SUCCESS:
            state.sendPin.action = authenticationConstants.SEND_PIN_TO_SMS_EMAIL_REQUEST_SENT;
            state.sendPin.payload = action.payload;
            state.sendPin.error = null;
            return;
        case authenticationConstants.SEND_PIN_TO_SMS_EMAIL_REQUEST_FAILED:
            state.sendPin.action = authenticationConstants.SEND_PIN_TO_SMS_EMAIL_REQUEST_SENT;
            state.sendPin.payload = null;
            state.sendPin.error = action.error;
            return;
        case authenticationConstants.FORGOT_PASSWORD_REQUEST_SENT:
            state.forgotPassword.action = authenticationConstants.FORGOT_PASSWORD_REQUEST_SENT;
            state.forgotPassword.payload = null;
            state.forgotPassword.error = null;
            return;
        case authenticationConstants.FORGOT_PASSWORD_REQUEST_SUCCESS:
            state.forgotPassword.action = authenticationConstants.FORGOT_PASSWORD_REQUEST_SUCCESS;
            state.forgotPassword.payload = action.payload;
            state.forgotPassword.error = null;
            return;
        case authenticationConstants.FORGOT_PASSWORD_REQUEST_FAILED:
            state.forgotPassword.action = authenticationConstants.FORGOT_PASSWORD_REQUEST_FAILED;
            state.forgotPassword.payload = null;
            state.forgotPassword.error = action.error;
            return;
        default:
            return;
    }
}, initialState);

export default reducer;