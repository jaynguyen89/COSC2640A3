import {IIssue, IResponse} from "../../../providers/helpers";
import {sendRequestForResult} from "../../../providers/serviceProvider";
import {IAuthUser} from "../../authentication/redux/interfaces";
import {IDataExport, IUpdateMarks} from "./interfaces";

const TEACHER_ENDPOINT = 'teacher/';

export const sendExportClassroomsRequest = (auth: IAuthUser, dataExport: IDataExport): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ TEACHER_ENDPOINT }export-classrooms`,
        auth,
        dataExport
    );
}

export const sendExportStudentsRequest = (auth: IAuthUser, dataExport: IDataExport): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ TEACHER_ENDPOINT }export-students`,
        auth,
        dataExport
    );
}

export const sendGetScheduleProgressRequest = (auth: IAuthUser): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ TEACHER_ENDPOINT }schedules`,
        auth,
        null,
        null,
        'GET'
    );
}

export const sendAddMarksToEnrolmentRequest = (auth: IAuthUser, marks: IUpdateMarks): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ TEACHER_ENDPOINT }add-marks`,
        auth,
        marks,
        null
    );
}