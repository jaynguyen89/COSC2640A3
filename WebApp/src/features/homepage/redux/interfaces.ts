import {IAuthUser} from "../../authentication/redux/interfaces";
import {
    invokeAddPhoneNumberRequest,
    invokeConfirmPhoneNumberRequest, invokeDisableTfaRequest, invokeEnableOrRenewTfaRequest,
    invokeGetStudentDetailRequest,
    invokeNewSmsTokenRequest,
    invokeRemovePhoneNumberRequest, invokeUpdateStudentRequest, invokeUpdateTeacherRequest
} from "./actions";
import {EMPTY_STRING, IActionResult} from "../../../providers/helpers";
import {
    clearAuthUser,
    invokeSignOutRequest,
    invokeSwitchRoleRequest,
    loadAuthUser
} from "../../authentication/redux/actions";

interface IAccount {
    email: string,
    username: string,
    preferredName: string,
    phoneNumber: string,
    phoneNumberConfirmed: boolean,
    twoFaEnabled: boolean,
    twoFa: ITwoFaDetail
}

interface ITwoFaDetail {
    qrImageUrl: string,
    manualQrCode: string
}

export interface IStudentDetail extends IAccount {
    studentId: string,
    schoolName: string,
    faculty: string,
    personalUrl: string
}

export interface ITeacherDetail extends IAccount {
    teacherId: string,
    company: string,
    jobTitle: string,
    personalWebsite: string
}

export interface IGenericHome {
    authUser: IAuthUser,
    invokeGetStudentDetailRequest: (auth: IAuthUser) => void,
    invokeGetTeacherDetailRequest: (auth: IAuthUser) => void,
    getStudentDetail: IActionResult,
    getTeacherDetail: IActionResult,
    clearAuthUser: () => void,
    invokeSwitchRoleRequest: (auth: IAuthUser) => void,
    switchRole: IActionResult,
    loadAuthUser: () => void
}

export const defaultStudent : IStudentDetail = {
    email: EMPTY_STRING,
    username: EMPTY_STRING,
    preferredName: EMPTY_STRING,
    phoneNumber: EMPTY_STRING,
    phoneNumberConfirmed: false,
    twoFaEnabled: false,
    twoFa: {
        qrImageUrl: EMPTY_STRING,
        manualQrCode: EMPTY_STRING
    },
    studentId: EMPTY_STRING,
    schoolName: EMPTY_STRING,
    faculty: EMPTY_STRING,
    personalUrl: EMPTY_STRING
}

export const defaultTeacher : ITeacherDetail = {
    email: EMPTY_STRING,
    username: EMPTY_STRING,
    preferredName: EMPTY_STRING,
    phoneNumber: EMPTY_STRING,
    phoneNumberConfirmed: false,
    twoFaEnabled: false,
    twoFa: {
        qrImageUrl: EMPTY_STRING,
        manualQrCode: EMPTY_STRING
    },
    teacherId: EMPTY_STRING,
    company: EMPTY_STRING,
    jobTitle: EMPTY_STRING,
    personalWebsite: EMPTY_STRING
}

export interface IStudentComponent {
    authUser: IAuthUser,
    studentDetail: IStudentDetail,
    updateStudent: IActionResult,
    invokeUpdateStudentRequest: (auth: IAuthUser, student: any) => void,
    clearAuthUser: () => void
}

export interface ITeacherComponent {
    authUser: IAuthUser,
    teacherDetail: ITeacherDetail,
    updateTeacher: IActionResult,
    invokeUpdateTeacherRequest: (auth: IAuthUser, teacher: any) => void,
    clearAuthUser: () => void
}

export interface IAccountDetail {
    authUser: IAuthUser,
    accountDetail: IStudentDetail | ITeacherDetail,
    invokeConfirmPhoneNumberRequest: (auth: IAuthUser, smsToken: string) => void,
    confirmPhoneNumber: IActionResult,
    invokeNewSmsTokenRequest: (auth: IAuthUser, recaptchaToken: string) => void,
    newSmsToken: IActionResult,
    invokeRemovePhoneNumberRequest: (auth: IAuthUser) => void,
    removePhoneNumber: IActionResult,
    invokeAddPhoneNumberRequest: (auth: IAuthUser, phoneNumber: string) => void,
    addPhoneNumber: IActionResult,
    clearAuthUser: () => void
}

export interface ITwoFactorDetail {
    authUser: IAuthUser,
    accountDetail: IStudentDetail | ITeacherDetail,
    invokeEnableOrRenewTfaRequest: (auth: IAuthUser) => void,
    invokeDisableTfaRequest: (auth: IAuthUser, recaptchaToken: string) => void,
    enableRenewTfa: IActionResult,
    disableTfa: IActionResult,
    clearAuthUser: () => void
}

export interface ITwoFa {
    qrImageUrl: string,
    manualQrCode: string
}

export const defaultTwoFa : ITwoFa = {
    qrImageUrl: EMPTY_STRING,
    manualQrCode: EMPTY_STRING
};