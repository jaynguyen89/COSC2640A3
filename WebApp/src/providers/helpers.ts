import _ from 'lodash';

export const EMPTY_STRING = '';
export const MAX_FILES_COUNT = 5;

export const modalOptions = {
    opacity : 0.8,
    inDuration : 200,
    outDuration : 200,
    dismissible : true
};

export interface IResponse {
    result: number,
    messages: Array<string>,
    data: Array<string> | Array<object> | object
}

export interface IIssue {
    type: string,
    title: string,
    status: number,
    traceId: string
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
    type : string
}

export const EMPTY_STATUS: IStatusMessage = {
    messages: [],
    type: EMPTY_STRING
}

export const isProperString = (item: string): boolean => {
    if (item === null) return false;

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
    const dtParts = dt.split('T');
    const dateParts = dtParts[0].split('-');
    const timeParts = dtParts[1].split(':');

    return `${ dateParts[2] }/${ dateParts[1] }/${ dateParts[0] } ${ timeParts[0] }:${ timeParts[1] }`;
}