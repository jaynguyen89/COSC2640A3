import * as accountConstants from './constants';
import produce from 'immer';
import {DEFAULT_ACTION_RESULT, IActionResult} from "../../../providers/helpers";

interface IAccountStore {
    getStudentDetail: IActionResult,
    getTeacherDetail: IActionResult,
    confirmPhoneNumber: IActionResult,
    removePhoneNumber: IActionResult,
    newSmsToken: IActionResult,
    addPhoneNumber: IActionResult,
    enableRenewTfa: IActionResult,
    disableTfa: IActionResult,
    updateStudent: IActionResult,
    updateTeacher: IActionResult
}

const initialState : IAccountStore = {
    getStudentDetail: DEFAULT_ACTION_RESULT,
    getTeacherDetail: DEFAULT_ACTION_RESULT,
    confirmPhoneNumber: DEFAULT_ACTION_RESULT,
    removePhoneNumber: DEFAULT_ACTION_RESULT,
    newSmsToken: DEFAULT_ACTION_RESULT,
    addPhoneNumber: DEFAULT_ACTION_RESULT,
    enableRenewTfa: DEFAULT_ACTION_RESULT,
    disableTfa: DEFAULT_ACTION_RESULT,
    updateStudent: DEFAULT_ACTION_RESULT,
    updateTeacher: DEFAULT_ACTION_RESULT
}

const reducer = produce((state, action) => {
    switch (action.type) {
        case accountConstants.GET_STUDENT_DETAIL_REQUEST_SENT:
            state.getStudentDetail.action = accountConstants.GET_STUDENT_DETAIL_REQUEST_SENT;
            state.getStudentDetail.payload = null;
            state.getStudentDetail.error = null;

            state.getTeacherDetail = DEFAULT_ACTION_RESULT;
            return;
        case accountConstants.GET_STUDENT_DETAIL_REQUEST_SUCCESS:
            state.getStudentDetail.action = accountConstants.GET_STUDENT_DETAIL_REQUEST_SUCCESS;
            state.getStudentDetail.payload = action.payload;
            state.getStudentDetail.error = null;
            return;
        case accountConstants.GET_STUDENT_DETAIL_REQUEST_FAILED:
            state.getStudentDetail.action = accountConstants.GET_STUDENT_DETAIL_REQUEST_FAILED;
            state.getStudentDetail.payload = null;
            state.getStudentDetail.error = action.error;
            return;
        case accountConstants.GET_TEACHER_DETAIL_REQUEST_SENT:
            state.getTeacherDetail.action = accountConstants.GET_TEACHER_DETAIL_REQUEST_SENT;
            state.getTeacherDetail.payload = null;
            state.getTeacherDetail.error = null;

            state.getStudentDetail = DEFAULT_ACTION_RESULT;
            return;
        case accountConstants.GET_TEACHER_DETAIL_REQUEST_SUCCESS:
            state.getTeacherDetail.action = accountConstants.GET_TEACHER_DETAIL_REQUEST_SUCCESS;
            state.getTeacherDetail.payload = action.payload;
            state.getTeacherDetail.error = null;
            return;
        case accountConstants.GET_TEACHER_DETAIL_REQUEST_FAILED:
            state.getTeacherDetail.action = accountConstants.GET_TEACHER_DETAIL_REQUEST_FAILED;
            state.getTeacherDetail.payload = null;
            state.getTeacherDetail.error = action.error;
            return;
        case accountConstants.CONFIRM_PHONE_NUMBER_REQUEST_SENT:
            state.confirmPhoneNumber.action = accountConstants.CONFIRM_PHONE_NUMBER_REQUEST_SENT;
            state.confirmPhoneNumber.payload = null;
            state.confirmPhoneNumber.error = null;
            return;
        case accountConstants.CONFIRM_PHONE_NUMBER_REQUEST_SUCCESS:
            state.confirmPhoneNumber.action = accountConstants.CONFIRM_PHONE_NUMBER_REQUEST_SUCCESS;
            state.confirmPhoneNumber.payload = action.payload;
            state.confirmPhoneNumber.error = null;
            return;
        case accountConstants.CONFIRM_PHONE_NUMBER_REQUEST_FAILED:
            state.confirmPhoneNumber.action = accountConstants.CONFIRM_PHONE_NUMBER_REQUEST_SUCCESS;
            state.confirmPhoneNumber.payload = null;
            state.confirmPhoneNumber.error = action.error;
            return;
        case accountConstants.REMOVE_PHONE_NUMBER_REQUEST_SENT:
            state.removePhoneNumber.action = accountConstants.REMOVE_PHONE_NUMBER_REQUEST_SENT;
            state.removePhoneNumber.payload = null;
            state.removePhoneNumber.error = null;
            return;
        case accountConstants.REMOVE_PHONE_NUMBER_REQUEST_SUCCESS:
            state.removePhoneNumber.action = accountConstants.REMOVE_PHONE_NUMBER_REQUEST_SUCCESS;
            state.removePhoneNumber.payload = action.payload;
            state.removePhoneNumber.error = null;
            return;
        case accountConstants.REMOVE_PHONE_NUMBER_REQUEST_FAILED:
            state.removePhoneNumber.action = accountConstants.REMOVE_PHONE_NUMBER_REQUEST_FAILED;
            state.removePhoneNumber.payload = null;
            state.removePhoneNumber.error = action.error;
            return;
        case accountConstants.NEW_SMS_TOKEN_REQUEST_SENT:
            state.newSmsToken.action = accountConstants.NEW_SMS_TOKEN_REQUEST_SENT;
            state.newSmsToken.payload = null;
            state.newSmsToken.error = null;
            return;
        case accountConstants.NEW_SMS_TOKEN_REQUEST_SUCCESS:
            state.newSmsToken.action = accountConstants.NEW_SMS_TOKEN_REQUEST_SUCCESS;
            state.newSmsToken.payload = action.payload;
            state.newSmsToken.error = null;
            return;
        case accountConstants.NEW_SMS_TOKEN_REQUEST_FAILED:
            state.newSmsToken.action = accountConstants.NEW_SMS_TOKEN_REQUEST_FAILED;
            state.newSmsToken.payload = null;
            state.newSmsToken.error = action.error;
            return;
        case accountConstants.ADD_PHONE_NUMBER_REQUEST_SENT:
            state.addPhoneNumber.action = accountConstants.ADD_PHONE_NUMBER_REQUEST_SENT;
            state.addPhoneNumber.payload = null;
            state.addPhoneNumber.error = null;
            return;
        case accountConstants.ADD_PHONE_NUMBER_REQUEST_SUCCESS:
            state.addPhoneNumber.action = accountConstants.ADD_PHONE_NUMBER_REQUEST_SUCCESS;
            state.addPhoneNumber.payload = action.payload;
            state.addPhoneNumber.error = null;
            return;
        case accountConstants.ADD_PHONE_NUMBER_REQUEST_FAILED:
            state.addPhoneNumber.action = accountConstants.ADD_PHONE_NUMBER_REQUEST_FAILED;
            state.addPhoneNumber.payload = null;
            state.addPhoneNumber.error = action.error;
            return;
        case accountConstants.ENABLE_RENEW_TFA_REQUEST_SENT:
            state.enableRenewTfa.action = accountConstants.ENABLE_RENEW_TFA_REQUEST_SENT;
            state.enableRenewTfa.payload = null;
            state.enableRenewTfa.error = null;
            return;
        case accountConstants.ENABLE_RENEW_TFA_REQUEST_SUCCESS:
            state.enableRenewTfa.action = accountConstants.ENABLE_RENEW_TFA_REQUEST_SUCCESS;
            state.enableRenewTfa.payload = action.payload;
            state.enableRenewTfa.error = null;
            return;
        case accountConstants.ENABLE_RENEW_TFA_REQUEST_FAILED:
            state.enableRenewTfa.action = accountConstants.ENABLE_RENEW_TFA_REQUEST_FAILED;
            state.enableRenewTfa.payload = null;
            state.enableRenewTfa.error = action.error;
            return;
        case accountConstants.DISABLE_TFA_REQUEST_SENT:
            state.disableTfa.action = accountConstants.DISABLE_TFA_REQUEST_SENT;
            state.disableTfa.payload = null;
            state.disableTfa.error = null;
            return;
        case accountConstants.DISABLE_TFA_REQUEST_SUCCESS:
            state.disableTfa.action = accountConstants.DISABLE_TFA_REQUEST_SUCCESS;
            state.disableTfa.payload = action.payload;
            state.disableTfa.error = null;
            return;
        case accountConstants.DISABLE_TFA_REQUEST_FAILED:
            state.disableTfa.action = accountConstants.DISABLE_TFA_REQUEST_FAILED;
            state.disableTfa.payload = null;
            state.disableTfa.error = action.error;
            return;
        case accountConstants.UPDATE_STUDENT_DETAIL_REQUEST_SENT:
            state.updateStudent.action = accountConstants.UPDATE_STUDENT_DETAIL_REQUEST_SENT;
            state.updateStudent.payload = null;
            state.updateStudent.error = null;
            return;
        case accountConstants.UPDATE_STUDENT_DETAIL_REQUEST_SUCCESS:
            state.updateStudent.action = accountConstants.UPDATE_STUDENT_DETAIL_REQUEST_SUCCESS;
            state.updateStudent.payload = action.payload;
            state.updateStudent.error = null;
            return;
        case accountConstants.UPDATE_STUDENT_DETAIL_REQUEST_FAILED:
            state.updateStudent.action = accountConstants.UPDATE_STUDENT_DETAIL_REQUEST_FAILED;
            state.updateStudent.payload = null;
            state.updateStudent.error = action.error;
            return;
        case accountConstants.UPDATE_TEACHER_DETAIL_REQUEST_SENT:
            state.updateTeacher.action = accountConstants.UPDATE_TEACHER_DETAIL_REQUEST_SENT;
            state.updateTeacher.payload = null;
            state.updateTeacher.error = null;
            return;
        case accountConstants.UPDATE_TEACHER_DETAIL_REQUEST_SUCCESS:
            state.updateTeacher.action = accountConstants.UPDATE_TEACHER_DETAIL_REQUEST_SUCCESS;
            state.updateTeacher.payload = action.payload;
            state.updateTeacher.error = null;
            return;
        case accountConstants.UPDATE_TEACHER_DETAIL_REQUEST_FAILED:
            state.updateTeacher.action = accountConstants.UPDATE_TEACHER_DETAIL_REQUEST_FAILED;
            state.updateTeacher.payload = null;
            state.updateTeacher.error = action.error;
            return;
        default:
            return;
    }
}, initialState);

export default reducer;