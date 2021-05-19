import * as studentConstants from './constants';
import produce from 'immer';
import {DEFAULT_ACTION_RESULT, IActionResult} from "../../../providers/helpers";

interface IStudentStore {
    getStudentEnrolments: IActionResult,
    enrolClassroom: IActionResult,
    unenrolClassroom: IActionResult
}

const initialState: IStudentStore = {
    getStudentEnrolments: DEFAULT_ACTION_RESULT,
    enrolClassroom: DEFAULT_ACTION_RESULT,
    unenrolClassroom: DEFAULT_ACTION_RESULT
}

const reducer = produce((state, action) => {
    switch (action.type) {
        case studentConstants.GET_STUDENT_ENROLMENTS_REQUEST_SENT:
            state.getStudentEnrolments.action = studentConstants.GET_STUDENT_ENROLMENTS_REQUEST_SENT;
            state.getStudentEnrolments.payload = null;
            state.getStudentEnrolments.error = null;
            return;
        case studentConstants.GET_STUDENT_ENROLMENTS_REQUEST_SUCCESS:
            state.getStudentEnrolments.action = studentConstants.GET_STUDENT_ENROLMENTS_REQUEST_SUCCESS;
            state.getStudentEnrolments.payload = action.payload;
            state.getStudentEnrolments.error = null;
            return;
        case studentConstants.GET_STUDENT_ENROLMENTS_REQUEST_FAILED:
            state.getStudentEnrolments.action = studentConstants.GET_STUDENT_ENROLMENTS_REQUEST_FAILED;
            state.getStudentEnrolments.payload = null;
            state.getStudentEnrolments.error = action.error
            return;
        case studentConstants.ENROL_INTO_CLASSROOM_REQUEST_SENT:
            state.enrolClassroom.action = studentConstants.ENROL_INTO_CLASSROOM_REQUEST_SENT;
            state.enrolClassroom.payload = null;
            state.enrolClassroom.error = null;
            return;
        case studentConstants.ENROL_INTO_CLASSROOM_REQUEST_SUCCESS:
            state.enrolClassroom.action = studentConstants.ENROL_INTO_CLASSROOM_REQUEST_SUCCESS;
            state.enrolClassroom.payload = action.payload;
            state.enrolClassroom.error = null;
            return;
        case studentConstants.ENROL_INTO_CLASSROOM_REQUEST_FAILED:
            state.enrolClassroom.action = studentConstants.ENROL_INTO_CLASSROOM_REQUEST_FAILED;
            state.enrolClassroom.payload = null;
            state.enrolClassroom.error = action.error
            return;
        case studentConstants.UNENROL_INTO_CLASSROOM_REQUEST_SENT:
            state.unenrolClassroom.action = studentConstants.UNENROL_INTO_CLASSROOM_REQUEST_SENT;
            state.unenrolClassroom.payload = null;
            state.unenrolClassroom.error = null;
            return;
        case studentConstants.UNENROL_INTO_CLASSROOM_REQUEST_SUCCESS:
            state.unenrolClassroom.action = studentConstants.UNENROL_INTO_CLASSROOM_REQUEST_SUCCESS;
            state.unenrolClassroom.payload = action.payload;
            state.unenrolClassroom.error = null;
            return;
        case studentConstants.UNENROL_INTO_CLASSROOM_REQUEST_FAILED:
            state.unenrolClassroom.action = studentConstants.UNENROL_INTO_CLASSROOM_REQUEST_FAILED;
            state.unenrolClassroom.payload = null;
            state.unenrolClassroom.error = action.error
            return;
        default:
            return;
    }
}, initialState);

export default reducer;