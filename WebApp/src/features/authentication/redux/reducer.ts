import * as authenticationConstants from './constants';
import produce from 'immer';
import {DEFAULT_ACTION_RESULT, EMPTY_STRING, IActionResult} from "../../../providers/helpers";
import {DEFAULT_AUTH_USER, IAuthUser} from "./interfaces";

interface IAuthenticationStore {
    authenticate: IActionResult,
    register: IActionResult,
    authUser: IAuthUser,
    unauthenticate: IActionResult
}

const initialState: IAuthenticationStore = {
    authenticate: DEFAULT_ACTION_RESULT,
    register: DEFAULT_ACTION_RESULT,
    authUser: DEFAULT_AUTH_USER,
    unauthenticate: DEFAULT_ACTION_RESULT
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
            state.authUser.email = action.payload.email;
            state.authUser.authToken = action.payload.authToken;
            state.authUser.accountId = action.payload.accountId;
            state.authUser.username = action.payload.username;
            return;
        case authenticationConstants.LOAD_AUTH_DATA:
            state.authUser.isAuthenticated = action.payload.isAuthenticated;
            state.authUser.email = action.payload.email;
            state.authUser.authToken = action.payload.authToken;
            state.authUser.accountId = action.payload.accountId;
            state.authUser.username = action.payload.username;
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
        default:
            return;
    }
}, initialState);

export default reducer;