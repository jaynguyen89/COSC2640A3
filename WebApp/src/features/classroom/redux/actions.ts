import {IAuthUser} from "../../authentication/redux/interfaces";
import * as classroomConstants from "./constants";
import * as classroomServices from "./services";

export const invokeGetAllTeacherClassroomsRequest = (auth: IAuthUser, teacherId: string | null) => {
    return (dispatch: any) => {
        dispatch({ type: classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_SENT });

        classroomServices.sendGetAllTeacherClassroomsRequest(auth, teacherId)
            .then(response => dispatch({
                type: classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeRemoveClassroomsRequest = (auth: IAuthUser, classroomId: string) => {
    return (dispatch: any) => {
        dispatch({ type: classroomConstants.REMOVE_CLASSROOMS_REQUEST_SENT });

        classroomServices.sendRemoveClassroomRequest(auth, classroomId)
            .then(response => dispatch({
                type: classroomConstants.REMOVE_CLASSROOMS_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: classroomConstants.REMOVE_CLASSROOMS_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeCompletedClassroomsRequest = (auth: IAuthUser, classroomId: string) => {
    return (dispatch: any) => {
        dispatch({ type: classroomConstants.COMPLETED_CLASSROOMS_REQUEST_SENT });

        classroomServices.sendCompletedClassroomRequest(auth, classroomId)
            .then(response => dispatch({
                type: classroomConstants.COMPLETED_CLASSROOMS_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: classroomConstants.COMPLETED_CLASSROOMS_REQUEST_FAILED,
                error
            }))
    };
}