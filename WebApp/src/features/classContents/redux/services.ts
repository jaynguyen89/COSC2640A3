import {IIssue, IResponse} from "../../../providers/helpers";
import {sendRequestForResult} from "../../../providers/serviceProvider";
import {IAuthUser} from "../../authentication/redux/interfaces";
import {IClassContent, IRichContent, IUpdateFile, IRichContentImport,IFileAdding} from "./interfaces"

const CLASSCONTENT_ENDPOINT = 'classcontent/';

export const sendAddFileRequest = (auth: IAuthUser, file: IFileAdding): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSCONTENT_ENDPOINT }add-files`,
        auth,
        null,
        file,
        'POST'
    );
}

export const sendUpdateFileRequest = (auth: IAuthUser, file: IUpdateFile): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSCONTENT_ENDPOINT }update-files`,
        auth,
        null,
        file,
        'POST'
    );
}

export const sendRichContentRequest = (auth: IAuthUser, richContent: IRichContent): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSCONTENT_ENDPOINT }add-rich-content`,
        auth,
        richContent,
        null,
        'POST'
    );
}

export const sendImportRichContentRequest = (auth: IAuthUser, importRichContent: IRichContentImport): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSCONTENT_ENDPOINT }import-rich-content`,
        auth,
        null,
        importRichContent,
        'POST'
    );
}

export const sendUpdateRichContentRequest = (auth: IAuthUser, richContent: IRichContent): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSCONTENT_ENDPOINT }update-rich-content`,
        auth,
        richContent,
        null,
        'PUT'
    );
}

export const sendAllRequest = (auth: IAuthUser, classroomId: string): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSCONTENT_ENDPOINT }all/${ classroomId }`,
        auth,
        null,
        null,
        'GET'
    );
}

