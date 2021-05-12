import * as accountConstants from './constants';
import * as accountServices from './services';
import {IAuthUser} from "../../authentication/redux/interfaces";

export const invokeGetStudentDetailRequest = (auth: IAuthUser) => {
    return (dispatch: any) => {
        dispatch({ type: accountConstants.GET_STUDENT_DETAIL_REQUEST_SENT });

        accountServices.sendGetStudentDetailRequest(auth)
            .then(response => dispatch({
                type: accountConstants.GET_STUDENT_DETAIL_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: accountConstants.GET_STUDENT_DETAIL_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeGetTeacherDetailRequest = (auth: IAuthUser) => {
    return (dispatch: any) => {
        dispatch({ type: accountConstants.GET_TEACHER_DETAIL_REQUEST_SENT });

        accountServices.sendGetTeacherDetailRequest(auth)
            .then(response => dispatch({
                type: accountConstants.GET_TEACHER_DETAIL_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: accountConstants.GET_TEACHER_DETAIL_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeConfirmPhoneNumberRequest = (auth: IAuthUser, smsToken: string) => {
    return (dispatch: any) => {
        dispatch({ type: accountConstants.CONFIRM_PHONE_NUMBER_REQUEST_SENT });

        accountServices.sendConfirmPhoneNumberRequest(auth, smsToken)
            .then(response => dispatch({
                type: accountConstants.CONFIRM_PHONE_NUMBER_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: accountConstants.CONFIRM_PHONE_NUMBER_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeNewSmsTokenRequest = (auth: IAuthUser, recaptchaToken: string) => {
    return (dispatch: any) => {
        dispatch({ type: accountConstants.NEW_SMS_TOKEN_REQUEST_SENT });

        accountServices.sendNewSmsTokenRequest(auth, recaptchaToken)
            .then(response => dispatch({
                type: accountConstants.NEW_SMS_TOKEN_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: accountConstants.NEW_SMS_TOKEN_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeRemovePhoneNumberRequest = (auth: IAuthUser) => {
    return (dispatch: any) => {
        dispatch({ type: accountConstants.REMOVE_PHONE_NUMBER_REQUEST_SENT });

        accountServices.sendRemovePhoneNumberRequest(auth)
            .then(response => dispatch({
                type: accountConstants.REMOVE_PHONE_NUMBER_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: accountConstants.REMOVE_PHONE_NUMBER_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeAddPhoneNumberRequest = (auth: IAuthUser, phoneNumber: string) => {
    return (dispatch: any) => {
        dispatch({ type: accountConstants.ADD_PHONE_NUMBER_REQUEST_SENT });

        accountServices.sendAddPhoneNumberRequest(auth, phoneNumber)
            .then(response => dispatch({
                type: accountConstants.ADD_PHONE_NUMBER_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: accountConstants.ADD_PHONE_NUMBER_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeEnableOrRenewTfaRequest = (auth: IAuthUser) => {
    return (dispatch: any) => {
        dispatch({ type: accountConstants.ENABLE_RENEW_TFA_REQUEST_SENT });

        accountServices.sendEnableOrRenewTfaRequest(auth)
            .then(response => dispatch({
                type: accountConstants.ENABLE_RENEW_TFA_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: accountConstants.ENABLE_RENEW_TFA_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeDisableTfaRequest = (auth: IAuthUser, recaptchaToken: string) => {
    return (dispatch: any) => {
        dispatch({ type: accountConstants.DISABLE_TFA_REQUEST_SENT });

        accountServices.sendDisableTfaRequest(auth, recaptchaToken)
            .then(response => dispatch({
                type: accountConstants.DISABLE_TFA_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: accountConstants.DISABLE_TFA_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeUpdateStudentRequest = (auth: IAuthUser, student: any) => {
    return (dispatch: any) => {
        dispatch({ type: accountConstants.UPDATE_STUDENT_DETAIL_REQUEST_SENT });

        accountServices.sendUpdateStudentRequest(auth, student)
            .then(response => dispatch({
                type: accountConstants.UPDATE_STUDENT_DETAIL_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: accountConstants.UPDATE_STUDENT_DETAIL_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeUpdateTeacherRequest = (auth: IAuthUser, teacher: any) => {
    return (dispatch: any) => {
        dispatch({ type: accountConstants.UPDATE_TEACHER_DETAIL_REQUEST_SENT });

        accountServices.sendUpdateTeacherRequest(auth, teacher)
            .then(response => dispatch({
                type: accountConstants.UPDATE_TEACHER_DETAIL_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: accountConstants.UPDATE_TEACHER_DETAIL_REQUEST_FAILED,
                error
            }))
    }
}