import {IIssue, IResponse} from "../../../providers/helpers";
import {IAuthUser} from "../../authentication/redux/interfaces";
import {sendRequestForResult} from "../../../providers/serviceProvider";

const ACCOUNT_ENDPOINT = 'account/';
const SECURITY_ENDPOINT = 'security/';

export const sendGetStudentDetailRequest = (auth: IAuthUser): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ ACCOUNT_ENDPOINT }student`,
        auth,
        null,
        null,
        'GET'
    );
}

export const sendGetTeacherDetailRequest = (auth: IAuthUser): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ ACCOUNT_ENDPOINT }teacher`,
        auth,
        null,
        null,
        'GET'
    );
}

export const sendConfirmPhoneNumberRequest = (auth: IAuthUser, smsToken: string): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ SECURITY_ENDPOINT }verify-sms-token/${ smsToken }`,
        auth,
        null,
        null,
        'GET'
    );
}

export const sendNewSmsTokenRequest = (auth: IAuthUser, recaptchaToken: string): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ SECURITY_ENDPOINT }request-sms-token/${ recaptchaToken }`,
        auth,
        null,
        null,
        'GET'
    );
}

export const sendRemovePhoneNumberRequest = (auth: IAuthUser): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ ACCOUNT_ENDPOINT }remove-phone-number`,
        auth,
        null,
        null,
        'PUT'
    );
}

export const sendAddPhoneNumberRequest = (auth: IAuthUser, phoneNumber: string): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ ACCOUNT_ENDPOINT }set-phone-number/${ phoneNumber }`,
        auth,
        null,
        null,
        'PUT'
    );
}

export const sendEnableOrRenewTfaRequest = (auth: IAuthUser): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ ACCOUNT_ENDPOINT }new-tfa`,
        auth,
        null,
        null,
        'PUT'
    );
}

export const sendDisableTfaRequest = (auth: IAuthUser, recaptchaToken: string): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ ACCOUNT_ENDPOINT }disable-tfa/${ recaptchaToken }`,
        auth,
        null,
        null,
        'PUT'
    );
}

export const sendUpdateStudentRequest = (auth: IAuthUser, student: any): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ ACCOUNT_ENDPOINT }update-student`,
        auth,
        student,
        null,
        'PUT'
    );
}

export const sendUpdateTeacherRequest = (auth: IAuthUser, teacher: any): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ ACCOUNT_ENDPOINT }update-teacher`,
        auth,
        teacher,
        null,
        'PUT'
    );
}