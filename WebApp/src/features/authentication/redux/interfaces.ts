import {EMPTY_STRING, IActionResult, IStatusMessage} from "../../../providers/helpers";
import {
    invokeAccountActivationRequest,
    invokeConfirmTfaPinRequest,
    invokeForgotPasswordRequest,
    invokeSendPinToSmsAndEmailRequest
} from "./actions";

export interface IAuthUser {
    isAuthenticated: boolean,
    authToken: string,
    accountId: string,
    role: number // 0 for student, 1 for teacher
}

export const DEFAULT_AUTH_USER: IAuthUser = {
    isAuthenticated: false,
    authToken: EMPTY_STRING,
    accountId: EMPTY_STRING,
    role: 0
};

export interface ILoginComponent {
    invokeAuthenticationRequest: (credentials: ICredentials) => void,
    setAuthUser: (authUser: IAuthUser) => void,
    authUser: IAuthUser,
    globalMessage: IStatusMessage,
    authentication: IActionResult
}

export interface IIdentity {
    email: string,
    username: string,
    recaptchaToken: string
}

export const defaultIdentity: IIdentity = {
    email: EMPTY_STRING,
    username: EMPTY_STRING,
    recaptchaToken: EMPTY_STRING
}

export interface ICredentials extends IIdentity {
    password: string,
    asStudent: boolean
}

export const defaultCredentials: ICredentials = {
    email: EMPTY_STRING,
    username: EMPTY_STRING,
    password: EMPTY_STRING,
    asStudent: true,
    recaptchaToken: EMPTY_STRING
};

export interface IRegistrationComponent {
    authUser: IAuthUser,
    registration: IActionResult,
    invokeRegistrationRequest: (accountData: IAccountData) => void
}

export interface IAccountData extends IIdentity {
    id: string,
    password: string,
    passwordConfirm: string,
    phoneNumber: string,
    preferredName: string
}

export const EMPTY_ACCOUNT: IAccountData = {
    id: EMPTY_STRING,
    email: EMPTY_STRING,
    username: EMPTY_STRING,
    password: EMPTY_STRING,
    passwordConfirm: EMPTY_STRING,
    phoneNumber: EMPTY_STRING,
    preferredName: EMPTY_STRING,
    recaptchaToken: EMPTY_STRING
}

export interface IConfirmTfa {
    authUser: IAuthUser,
    invokeConfirmTfaPinRequest: (auth: IAuthUser, pin: string) => void,
    confirmTfa: IActionResult,
    setAuthUser: (authUser: IAuthUser) => void,
    invokeSendPinToSmsAndEmailRequest: (auth: IAuthUser) => void,
    sendPin: IActionResult
}

export interface IActivateAccount {
    authUser: IAuthUser,
    invokeAccountActivationRequest: (activationData: IActivationData) => void,
    activateAccount: IActionResult
}

export interface IActivationData extends IIdentity {
    confirmCode: string
}

export const defaultActivationData : IActivationData = {
    email: EMPTY_STRING,
    username: EMPTY_STRING,
    confirmCode: EMPTY_STRING,
    recaptchaToken: EMPTY_STRING
}

export interface IForgotPassword {
    authUser: IAuthUser,
    invokeForgotPasswordRequest: (identity: IIdentity) => void,
    forgotPassword: IActionResult
}

export interface IResetPassword {
    resetPassword: IActionResult,
    invokePasswordResetRequest: (data: IPasswordReset) => void
}

export interface IPasswordReset {
    accountId: string,
    recoveryToken: string,
    password: string,
    passwordConfirm: string,
    recaptchaToken: string
}

export const defaultPasswordReset: IPasswordReset = {
    accountId: EMPTY_STRING,
    recoveryToken: EMPTY_STRING,
    password: EMPTY_STRING,
    passwordConfirm: EMPTY_STRING,
    recaptchaToken: EMPTY_STRING
}