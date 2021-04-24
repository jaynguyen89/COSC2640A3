import * as authenticationConstants from './constants';
import * as authenticationServices from './services';
import {IAccountData, IAuthUser, ICredentials} from "./interfaces";

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

export const setAuthUser = (authUser: object) => {
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