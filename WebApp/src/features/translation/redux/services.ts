import {sendRequestForResult} from "../../../providers/serviceProvider";
import {IAuthUser} from "../../authentication/redux/interfaces";
import {ITranslateIt} from "./interfaces";
import {IIssue, IResponse} from "../../../providers/helpers";

const CLASSROOM_CONTENT_ENDPOINT = 'class-content/';

export const sendTranslateSentenceRequest = (auth: IAuthUser, data: ITranslateIt): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSROOM_CONTENT_ENDPOINT }translate-sentences`,
        auth,
        data
    );
}

export const sendTranslateWordRequest = (auth: IAuthUser, data: ITranslateIt): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSROOM_CONTENT_ENDPOINT }translate-word`,
        auth,
        data
    );
}

export const sendGetThesaurusRequest = (auth: IAuthUser, data: ITranslateIt): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSROOM_CONTENT_ENDPOINT }get-thesaurus`,
        auth,
        data
    );
}