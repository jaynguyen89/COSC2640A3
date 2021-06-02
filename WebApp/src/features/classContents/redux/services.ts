import {IIssue, IResponse} from "../../../providers/helpers";
import {sendRequestForResult} from "../../../providers/serviceProvider";
import {IAuthUser} from "../../authentication/redux/interfaces";
import {IFileAdding, IFileUpdating, IRichContent, IRichContentImport} from "./interfaces";

const CLASSROOM_CONTENT_ENDPOINT = 'class-content/';

export const sendGetClassroomContentRequest = (auth: IAuthUser, classroomId: string): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSROOM_CONTENT_ENDPOINT }all/${ classroomId }`,
        auth,
        null,
        null,
        'GET'
    );
}

export const sendAddFilesRequest = (auth: IAuthUser, filesData: IFileAdding): Promise<IResponse | IIssue> => {
    let formData = new FormData();

    formData.append('classroomId', filesData.classroomId);
    formData.append('fileType', filesData.fileType.toString());

    for (let i = 0; i < filesData.uploadedFiles.length; i++)
        formData.append('uploadedFiles', filesData.uploadedFiles[i], filesData.uploadedFiles[i].name);

    return sendRequestForResult(
        `${ CLASSROOM_CONTENT_ENDPOINT }add-files`,
        auth,
        null,
        formData
    );
}

export const sendUpdateFilesRequest = (auth: IAuthUser, filesData: IFileUpdating): Promise<IResponse | IIssue> => {
    let formData = new FormData();

    formData.append('classroomId', filesData.classroomId);
    formData.append('fileType', filesData.fileType.toString());
    formData.append('removedFiles', JSON.stringify(filesData.removedFiles));

    if (filesData.uploadedFiles)
        for (let i = 0; i < filesData.uploadedFiles.length; i++)
            formData.append('uploadedFiles', filesData.uploadedFiles[i], filesData.uploadedFiles[i].name);

    return sendRequestForResult(
        `${ CLASSROOM_CONTENT_ENDPOINT }update-files`,
        auth,
        null,
        formData
    );
}

export const sendAddRichContentRequest = (auth: IAuthUser, richContent: IRichContent): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSROOM_CONTENT_ENDPOINT }add-rich-content`,
        auth,
        richContent
    );
}

export const sendImportRichContentRequest = (auth: IAuthUser, richContentImport: IRichContentImport): Promise<IResponse | IIssue> => {
    let formData = new FormData();

    formData.append('classroomId', richContentImport.classroomId);

    for (let i = 0; i < richContentImport.filesForImport.length; i++)
        formData.append('filesForImport', richContentImport.filesForImport[i], richContentImport.filesForImport[i].name);

    return sendRequestForResult(
        `${ CLASSROOM_CONTENT_ENDPOINT }import-rich-content`,
        auth,
        null,
        formData
    );
}

export const sendUpdateRichContentRequest = (auth: IAuthUser, richContent: IRichContent): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ CLASSROOM_CONTENT_ENDPOINT }update-rich-content`,
        auth,
        richContent,
        null,
        'PUT'
    );
}