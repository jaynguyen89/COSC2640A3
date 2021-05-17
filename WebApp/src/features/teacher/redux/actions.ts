import * as teacherConstants from './constants';
import * as teacherServices from './services';
import {IAuthUser} from "../../authentication/redux/interfaces";
import {IDataExport} from "./interfaces";

export const invokeExportClassroomsRequest = (auth: IAuthUser, dataExport: IDataExport) => {
    return (dispatch: any) => {
        dispatch({ type: teacherConstants.EXPORT_CLASSROOMS_REQUEST_SENT });

        teacherServices.sendExportClassroomsRequest(auth, dataExport)
            .then(response => dispatch({
                type: teacherConstants.EXPORT_CLASSROOMS_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: teacherConstants.EXPORT_CLASSROOMS_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeExportStudentsRequest = (auth: IAuthUser, dataExport: IDataExport) => {
    return (dispatch: any) => {
        dispatch({ type: teacherConstants.EXPORT_STUDENTS_REQUEST_SENT });

        teacherServices.sendExportStudentsRequest(auth, dataExport)
            .then(response => dispatch({
                type: teacherConstants.EXPORT_STUDENTS_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: teacherConstants.EXPORT_STUDENTS_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeGetSchedulesProgressRequest = (auth: IAuthUser) => {
    return (dispatch: any) => {
        dispatch({ type: teacherConstants.GET_IMPORT_SCHEDULES_PROGRESS_REQUEST_SENT });

        teacherServices.sendGetScheduleProgressRequest(auth)
            .then(response => dispatch({
                type: teacherConstants.GET_IMPORT_SCHEDULES_PROGRESS_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: teacherConstants.GET_IMPORT_SCHEDULES_PROGRESS_REQUEST_FAILED,
                error
            }))
    };
}