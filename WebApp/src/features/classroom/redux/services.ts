import {IIssue, IResponse} from "../../../providers/helpers";
import {sendRequestForResult} from "../../../providers/serviceProvider";
import {IAuthUser} from "../../authentication/redux/interfaces";

const CLASSROOM_ENDPOINT = 'classroom/';

export const sendGetAllTeacherClassroomsRequest = (auth: IAuthUser, teacherId: string | null): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        (
            teacherId && `${ CLASSROOM_ENDPOINT }all-by-teacher/${ teacherId }`
        ) ||
        `${ CLASSROOM_ENDPOINT }all-by-teacher`,
        auth,
        null,
        null,
        'GET'
    );
}

export const sendRemoveClassroomRequest = (auth: IAuthUser, classroomId: string): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSROOM_ENDPOINT }remove/${ classroomId }`,
        auth,
        null,
        null,
        'DELETE'
    );
}

export const sendCompletedClassroomRequest = (auth: IAuthUser, classroomId: string): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSROOM_ENDPOINT }completed/${ classroomId }`,
        auth,
        null,
        null,
        'PUT'
    );
}