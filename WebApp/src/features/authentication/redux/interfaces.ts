import {EMPTY_STRING, IActionResult, IStatusMessage} from "../../../providers/helpers";

export interface IAuthUser {
    isAuthenticated: boolean,
    email: string,
    authToken: string,
    accountId: string,
    username: string
}

export const DEFAULT_AUTH_USER: IAuthUser = {
    isAuthenticated: false,
    email: EMPTY_STRING,
    authToken: EMPTY_STRING,
    accountId: EMPTY_STRING,
    username: EMPTY_STRING
};

export interface ILoginComponent {
    invokeAuthenticationRequest: (credentials: ICredentials) => void,
    setAuthUser: (authUser: object) => void,
    authUser: IAuthUser,
    globalMessage: IStatusMessage,
    authentication: IActionResult
}

export interface ICredentials {
    email: string,
    password: string
}

export const defaultCredentials: ICredentials = {
    email: EMPTY_STRING,
    password: EMPTY_STRING
};

export interface IRegistrationComponent {
    authUser: IAuthUser,
    registration: IActionResult,
    invokeRegistrationRequest: (accountData: IAccountData) => void
}

export interface IAccountData {
    id: string,
    email: string,
    username: string,
    password: string,
    passwordConfirm: string
}

export const EMPTY_ACCOUNT: IAccountData = {
    id: EMPTY_STRING,
    email: EMPTY_STRING,
    username: EMPTY_STRING,
    password: EMPTY_STRING,
    passwordConfirm: EMPTY_STRING
}