import _ from 'lodash';
import {IEnrolment, IMarkBreakdown} from "../features/student/redux/interfaces";
import * as studentConstants from '../features/student/redux/constants';
import {IFile} from "../features/classContents/redux/interfaces";

export const EMPTY_STRING = '';
export const TASK_VIEW = 'view';
export const TASK_UPDATE = 'update';
export const TASK_CREATE = 'create';

export const modalOptions = {
    opacity : 0.8,
    inDuration : 200,
    outDuration : 200,
    dismissible : true
};

export const DurationUnits = [
    { index: 0, text: 'Hours' },
    { index: 1, text: 'Days' },
    { index: 2, text: 'Weeks' },
    { index: 3, text: 'Months' },
    { index: 4, text: 'Years' }
]

export interface IResponse {
    result: number,
    messages: Array<string>,
    data: Array<string> | Array<object> | object
}

export interface IIssue {
    type: string,
    title: string,
    status: number,
    traceId: string,
    message: string
}

export interface IActionResult {
    action: string,
    payload: IResponse | null,
    error: IIssue | null
}

export const DEFAULT_ACTION_RESULT: IActionResult = {
    action: EMPTY_STRING,
    payload: null,
    error: null
}

export interface IStatusMessage {
    messages : Array<string>,
    type : string,
    closeAlert?: () => void,
}

export const EMPTY_STATUS: IStatusMessage = {
    messages: [],
    type: EMPTY_STRING
}

export const TimeUnits = ['Hour', 'Day', 'Week', 'Month', 'Year'];
export const FileTypes = ['video', 'audio', 'image', 'other'];
export const BaseFileUrl = 'https://s3-ap-southeast-2.amazonaws.com/';

export const createFileUrl = (file: IFile, classroomId: string) => {
    return `${ BaseFileUrl }${ classroomId.toLowerCase() }.${ FileTypes[file.type] }s/${ file.id }`;
}

export const isProperString = (item: string): boolean => {
    if (item === null || item === undefined) return false;

    item = _.trim(item);
    return item.length !== 0;
}

export const setGlobalMessage = (message: IStatusMessage) => {
    sessionStorage.setItem('globalMessage', JSON.stringify(message));
}

export const removeGlobalMessage = () => {
    sessionStorage.removeItem('globalMessage');
}

export const normalizeDt = (dt: string): string => {
    if (!isProperString(dt)) return EMPTY_STRING;

    const dtParts = dt.split('T');
    const dateParts = dtParts[0].split('-');
    const timeParts = dtParts[1].split(':');

    return `${ dateParts[2] }/${ dateParts[1] }/${ dateParts[0] } ${ timeParts[0] }:${ timeParts[1] }`;
}

export const checkSession = (
    clearAuthUser: () => void, setStatusMessage: (message: IStatusMessage) => void, error?: string
): boolean => {
    if (error === undefined || error.indexOf('401') === -1) {
        setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
        return true;
    }

    if (error.indexOf('401') !== -1) {
        setGlobalMessage({ messages: ['Your session has expired. Please login again.'], type: 'error' } as IStatusMessage);
        clearAuthUser();
        window.location.href = '/';
        return false;
    }

    return true;
}

export const calculateOverallMarks = (marks: Array<IMarkBreakdown>): number => {
    const totalSum = 1;
    const rewardedSum = 1;

    const average = totalSum * 100.0 / rewardedSum;
    return average % 1 >= 0.5 ? Math.ceil(average) : Math.floor(average);
}

export const checkUnenrolmentResult = (
    clearAuthUser: () => void, setStatusMessage: (message: IStatusMessage) => void, unenrolResult: IActionResult, enrolments: Array<IEnrolment>
): Array<IEnrolment> | undefined => {
    if (unenrolResult.action === studentConstants.UNENROL_INTO_CLASSROOM_REQUEST_FAILED)
        checkSession(clearAuthUser, setStatusMessage, unenrolResult.error?.message);

    if (unenrolResult.action === studentConstants.UNENROL_INTO_CLASSROOM_REQUEST_SUCCESS)
        if (unenrolResult.payload === null)
            setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
        else if (unenrolResult.payload.result === 0)
            setStatusMessage({ messages: unenrolResult.payload.messages, type: 'error' } as IStatusMessage);
        else {
            setStatusMessage({ messages: ['Your enrolment and its corresponding invoice has been removed.'], type: 'error' } as IStatusMessage);
            const removedEnrolmentId = unenrolResult.payload.data as unknown as string;

            let clone = _.cloneDeep(enrolments);
            _.remove(clone, enrolment => enrolment.id === removedEnrolmentId);
            return clone;
        }

    return undefined;
}

export const verifyFileTypeForSelectedFiles = (files: FileList, fileType: number): string | null => {
    if (fileType === 3) return null;

    let clone: Array<{ name: string, type: number }> = [];
    for (let i = 0; i < files.length; i++)
        clone.push({
            name: files[i].name,
            type: FileTypes.indexOf(files[i].type.split('/')[0])
        });

    const oddFiles = _.remove(clone, (file: { name: string, type: number }) => file.type !== fileType);
    if (oddFiles.length === 0) return null;

    return `The following files are not of type ${ FileTypes[fileType] }: ${ oddFiles.map(file => `${ file.name }`).join(', ') }`;
}