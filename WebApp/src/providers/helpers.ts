import _ from 'lodash';

export const EMPTY_STRING = '';
export const MAX_FILES_COUNT = 5;
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