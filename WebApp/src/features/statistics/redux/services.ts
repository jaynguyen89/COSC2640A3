import {IIssue, IResponse} from "../../../providers/helpers";
import {sendRequestForResult} from "../../../providers/serviceProvider";

const DATA_ENDPOINT = 'data/';

export const sendGetStatisticsRequest = (): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ DATA_ENDPOINT }get-statistics`,
        null,
        null,
        null,
        'GET'
    );
}

export const sendTriggerMapperRequest = (): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ DATA_ENDPOINT }trigger-emr-mapper`,
        null,
        null,
        null,
        'GET'
    );
}

export const sendTriggerReducerRequest = (): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ DATA_ENDPOINT }trigger-emr-reducer`,
        null,
        null,
        null,
        'GET'
    );
}