import {IIssue, IResponse} from "../../../providers/helpers";
import {sendRequestForResult} from "../../../providers/serviceProvider";
import {IAuthUser} from "../../authentication/redux/interfaces";

const STUDENT_ENDPOINT = 'student/';

export const sendGetStudentEnrolmentsRequest = (auth: IAuthUser): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ STUDENT_ENDPOINT }enrolments`,
        auth,
        null,
        null,
        'GET'
    );
};

export const sendEnrolIntoClassroomRequest = (auth: IAuthUser, classroomId: string): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ STUDENT_ENDPOINT }enrol/${ classroomId }`,
        auth,
        null
    );
}

export const sendUnenrolFromClassroomRequest = (auth: IAuthUser, enrolmentId: string): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ STUDENT_ENDPOINT }unenrol/${ enrolmentId }`,
        auth,
        null,
        null,
        'DELETE'
    );
}