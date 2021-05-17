import {IAuthUser} from "../../authentication/redux/interfaces";
import * as classroomConstants from "./constants";
import * as classroomServices from "./services";
import {IClassroom, IFileImport} from "./interfaces";

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
        dispatch({ type: classroomConstants.REMOVE_CLASSROOM_REQUEST_SENT });

        classroomServices.sendRemoveClassroomRequest(auth, classroomId)
            .then(response => dispatch({
                type: classroomConstants.REMOVE_CLASSROOM_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: classroomConstants.REMOVE_CLASSROOM_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeCompletedClassroomsRequest = (auth: IAuthUser, classroomId: string) => {
    return (dispatch: any) => {
        dispatch({ type: classroomConstants.COMPLETED_CLASSROOM_REQUEST_SENT });

        classroomServices.sendCompletedClassroomRequest(auth, classroomId)
            .then(response => dispatch({
                type: classroomConstants.COMPLETED_CLASSROOM_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: classroomConstants.COMPLETED_CLASSROOM_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeGetClassroomDetailRequest = (auth: IAuthUser, classroomId: string) => {
    return (dispatch: any) => {
        dispatch({ type: classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_SENT });

        classroomServices.sendGetClassroomDetailRequest(auth, classroomId)
            .then(response => dispatch({
                type: classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeCreateClassroomsRequest = (auth: IAuthUser, classroom: IClassroom) => {
    return (dispatch: any) => {
        dispatch({ type: classroomConstants.CREATE_CLASSROOM_REQUEST_SENT });

        classroomServices.sendCreateClassroomRequest(auth, classroom)
            .then(response => dispatch({
                type: classroomConstants.CREATE_CLASSROOM_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: classroomConstants.CREATE_CLASSROOM_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeUpdateClassroomsRequest = (auth: IAuthUser, classroom: IClassroom) => {
    return (dispatch: any) => {
        dispatch({ type: classroomConstants.UPDATE_CLASSROOM_REQUEST_SENT });

        classroomServices.sendUpdateClassroomRequest(auth, classroom)
            .then(response => dispatch({
                type: classroomConstants.UPDATE_CLASSROOM_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: classroomConstants.UPDATE_CLASSROOM_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeUploadFileForImportRequest = (auth: IAuthUser, fileData: IFileImport) => {
    return (dispatch: any) => {
        dispatch({ type: classroomConstants.UPLOAD_JSON_CLASSROOMS_FILE_REQUEST_SENT });

        classroomServices.sendImportJsonFileClassroomsRequest(auth, fileData)
            .then(response => dispatch({
                type: classroomConstants.UPLOAD_JSON_CLASSROOMS_FILE_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: classroomConstants.UPLOAD_JSON_CLASSROOMS_FILE_REQUEST_FAILED,
                error
            }))
    };
}