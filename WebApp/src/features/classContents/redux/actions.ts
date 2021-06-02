import * as contentConstants from './constants';
import * as contentServices from './services';
import {IAuthUser} from "../../authentication/redux/interfaces";
import {IFileAdding, IFileUpdating, IRichContent, IRichContentImport} from "./interfaces";

export const invokeGetClassroomContentRequest = (auth: IAuthUser, classroomId: string) => {
    return (dispatch: any) => {
        dispatch({ type: contentConstants.GET_CLASSROOM_CONTENTS_REQUEST_SENT });

        contentServices.sendGetClassroomContentRequest(auth, classroomId)
            .then(response => dispatch({
                type: contentConstants.GET_CLASSROOM_CONTENTS_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: contentConstants.GET_CLASSROOM_CONTENTS_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeAddFilesRequest = (auth: IAuthUser, filesData: IFileAdding) => {
    return (dispatch: any) => {
        dispatch({ type: contentConstants.ADD_FILES_TO_CLASSROOM_REQUEST_SENT });

        contentServices.sendAddFilesRequest(auth, filesData)
            .then(response => dispatch({
                type: contentConstants.ADD_FILES_TO_CLASSROOM_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: contentConstants.ADD_FILES_TO_CLASSROOM_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeUpdateFilesRequest = (auth: IAuthUser, filesData: IFileUpdating) => {
    return (dispatch: any) => {
        dispatch({ type: contentConstants.UPDATE_OR_REMOVE_FILES_REQUEST_SENT });

        contentServices.sendUpdateFilesRequest(auth, filesData)
            .then(response => dispatch({
                type: contentConstants.UPDATE_OR_REMOVE_FILES_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: contentConstants.UPDATE_OR_REMOVE_FILES_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeAddRichContentRequest = (auth: IAuthUser, richContent: IRichContent) => {
    return (dispatch: any) => {
        dispatch({ type: contentConstants.ADD_RICH_CONTENT_REQUEST_SENT });

        contentServices.sendAddRichContentRequest(auth, richContent)
            .then(response => dispatch({
                type: contentConstants.ADD_RICH_CONTENT_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: contentConstants.ADD_RICH_CONTENT_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeImportRichContentRequest = (auth: IAuthUser, richContentImport: IRichContentImport) => {
    return (dispatch: any) => {
        dispatch({ type: contentConstants.IMPORT_RICH_CONTENT_REQUEST_SENT });

        contentServices.sendImportRichContentRequest(auth, richContentImport)
            .then(response => dispatch({
                type: contentConstants.IMPORT_RICH_CONTENT_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: contentConstants.IMPORT_RICH_CONTENT_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeUpdateRichContentRequest = (auth: IAuthUser, richContent: IRichContent) => {
    return (dispatch: any) => {
        dispatch({ type: contentConstants.UPDATE_RICH_CONTENT_REQUEST_SENT });

        contentServices.sendUpdateRichContentRequest(auth, richContent)
            .then(response => dispatch({
                type: contentConstants.UPDATE_RICH_CONTENT_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: contentConstants.UPDATE_RICH_CONTENT_REQUEST_FAILED,
                error
            }))
    };
}