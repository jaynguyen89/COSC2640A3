import * as studentConstants from './constants';
import * as studentServices from './services';
import {IAuthUser} from "../../authentication/redux/interfaces";

export const invokeGetStudentEnrolmentsRequest = (auth: IAuthUser) => {
    return (dispatch: any) => {
        dispatch({ type: studentConstants.GET_STUDENT_ENROLMENTS_REQUEST_SENT });

        studentServices.sendGetStudentEnrolmentsRequest(auth)
            .then(response => dispatch({
                type: studentConstants.GET_STUDENT_ENROLMENTS_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: studentConstants.GET_STUDENT_ENROLMENTS_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeStudentEnrolmentRequest = (auth: IAuthUser, classroomId: string) => {
    return (dispatch: any) => {
        dispatch({ type: studentConstants.ENROL_INTO_CLASSROOM_REQUEST_SENT });

        studentServices.sendEnrolIntoClassroomRequest(auth, classroomId)
            .then(response => dispatch({
                type: studentConstants.ENROL_INTO_CLASSROOM_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: studentConstants.ENROL_INTO_CLASSROOM_REQUEST_FAILED,
                error
            }))
    }
}

export const invokeUnenrolFromClassroomRequest = (auth: IAuthUser, enrolmentId: string) => {
    return (dispatch: any) => {
        dispatch({ type: studentConstants.UNENROL_INTO_CLASSROOM_REQUEST_SENT });

        studentServices.sendUnenrolFromClassroomRequest(auth, enrolmentId)
            .then(response => dispatch({
                type: studentConstants.UNENROL_INTO_CLASSROOM_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: studentConstants.UNENROL_INTO_CLASSROOM_REQUEST_FAILED,
                error
            }))
    }
}