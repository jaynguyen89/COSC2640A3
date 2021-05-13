import { sendRequestForResult } from '../../../providers/serviceProvider';
import {DEFAULT_AUTH_USER, IAccountData, IActivationData, IAuthUser, ICredentials, IIdentity} from "./interfaces";
import {IIssue, IResponse, isProperString} from "../../../providers/helpers";

const AUTHENTICATION_ENDPOINT = 'authentication/';
const SECURITY_ENDPOINT = 'security/';

export const sendLoginRequest = (credentials: ICredentials): Promise<IResponse> => {
    return sendRequestForResult(
        `${ AUTHENTICATION_ENDPOINT }authenticate`,
        null,
        credentials
    );
}

export const loadAuthUserFromCookies = (): IAuthUser => {
    const authToken = localStorage.getItem('authToken');
    const accountId = localStorage.getItem('accountId');
    const role = localStorage.getItem('role');

    const isAuthenticated = isProperString(accountId as string) &&
                            isProperString(authToken as string) &&
                            isProperString(role as string);

    return isAuthenticated
        ? {
            isAuthenticated,
            authToken: authToken as string,
            accountId: accountId as string,
            role: Number(role as string)
        } as IAuthUser
        : DEFAULT_AUTH_USER;
}

export const clearAuthUserInCookie = (): void => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('accountId');
    localStorage.removeItem('role');
    localStorage.removeItem('preferredName');
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

export const sendConfirmTfaPinRequest = (authUser: IAuthUser, pin: string): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ SECURITY_ENDPOINT }confirm-tfa-pin/${ pin }`,
        authUser,
        null,
        null,
        'GET');
}

export const sendAccountActivationRequest = (activationData: IActivationData): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ AUTHENTICATION_ENDPOINT }unauthenticate`,
        null,
        activationData,
        null,
        'PUT'
    );
}

export const sendPinToSmsAndEmailRequest = (authUser: IAuthUser): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ SECURITY_ENDPOINT }send-tfa-pin`,
        authUser,
        null,
        null,
        'GET'
    );
}

export const sendForgotPasswordRequest = (identity: IIdentity): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ SECURITY_ENDPOINT }request-recovery-token`,
        null,
        identity
    );
}

export const sendSwitchRoleRequest = (auth: IAuthUser): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ AUTHENTICATION_ENDPOINT }switch-role`,
        auth,
        null,
        null,
        'GET'
    );
}