import {IIssue, IResponse} from "../../../providers/helpers";
import {sendRequestForResult} from "../../../providers/serviceProvider";
import {IAuthUser} from "../../authentication/redux/interfaces";
import {CLASSROOMS_PER_PAGE, IClassroom, IFileImport} from "./interfaces";

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

export const sendGetClassroomDetailRequest = (auth: IAuthUser, classroomId: string): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSROOM_ENDPOINT }details/${ classroomId }`,
        auth,
        null,
        null,
        'GET'
    );
}

export const sendCreateClassroomRequest = (auth: IAuthUser, classroom: IClassroom): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSROOM_ENDPOINT }create`,
        auth,
        classroom,
        null
    );
}

export const sendUpdateClassroomRequest = (auth: IAuthUser, classroom: IClassroom): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSROOM_ENDPOINT }update`,
        auth,
        classroom,
        null,
        'PUT'
    );
}

export const sendImportJsonFileClassroomsRequest = (auth: IAuthUser, fileData: IFileImport): Promise<IResponse | IIssue> => {
    let formData = new FormData();

    formData.append('importType', fileData.importType.toString());
    formData.append('fileForImport', fileData.fileForImport, fileData.fileForImport.name);

    return sendRequestForResult(
        `${ CLASSROOM_ENDPOINT }import`,
        auth,
        null,
        formData
    );
};

export const sendGetAllClassroomsRequest = (auth: IAuthUser, offset: number): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSROOM_ENDPOINT }all/${ offset }/${ CLASSROOMS_PER_PAGE }`,
        auth,
        null,
        null,
        'GET'
    );
}

export const sendGetEnrolmentsByClassroomRequest = (auth: IAuthUser, classroomId: string): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSROOM_ENDPOINT }enrolments/${ classroomId }`,
        auth,
        null,
        null,
        'GET'
    );
}

export const sendSearchClassroomsRequest = (
    auth: IAuthUser, searchData: { classroomName: string, teacherName: string }
): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSROOM_ENDPOINT }search`,
        auth,
        searchData,
    );
}