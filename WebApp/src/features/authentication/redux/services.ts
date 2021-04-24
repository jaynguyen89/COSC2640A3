import { sendRequestForResult } from '../../../providers/serviceProvider';
import {DEFAULT_AUTH_USER, IAccountData, IAuthUser, ICredentials} from "./interfaces";
import {IIssue, IResponse, isProperString} from "../../../providers/helpers";

const AUTHENTICATION_ENDPOINT = 'authentication/';

export const sendLoginRequest = (credentials: ICredentials): Promise<IResponse> => {
    return sendRequestForResult(
        `${ AUTHENTICATION_ENDPOINT }authenticate`,
        null,
        credentials
    );
}

export const loadAuthUserFromCookies = (): IAuthUser => {
    const email = localStorage.getItem('email');
    const authToken = localStorage.getItem('authToken');
    const accountId = localStorage.getItem('accountId');
    const username = localStorage.getItem('username');

    const isAuthenticated = isProperString(email as string) && isProperString(authToken as string) &&
        isProperString(accountId as string) && isProperString(username as string);

    return isAuthenticated
        ? {
            isAuthenticated,
            email,
            authToken,
            accountId,
            username
        } as IAuthUser
        : DEFAULT_AUTH_USER;
}

export const clearAuthUserInCookie = (): void => {
    localStorage.removeItem('email');
    localStorage.removeItem('authToken');
    localStorage.removeItem('accountId');
    localStorage.removeItem('username');
}

export const sendRegistrationRequest = (accountData: IAccountData): Promise<IResponse> => {
    return sendRequestForResult(
        `${ AUTHENTICATION_ENDPOINT }register`,
        null,
        accountData
    );
}

export const sendSignOutRequest = (authUser: IAuthUser): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ AUTHENTICATION_ENDPOINT }unauthenticate`,
        authUser,
        null,
        null,
        'GET'
    );
}