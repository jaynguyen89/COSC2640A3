import {IAuthUser} from "../../authentication/redux/interfaces";
import * as classcontentConstants from "./constants";
import * as classcontentServices from "./services";
import {IClassContent, IFileAdding, IRichContent,IUpdateFile} from "./interfaces";


export const invokeAddFileRequest = (auth: IAuthUser, file:IFileAdding) => {
    return (dispatch: any) => {
        dispatch({ type: classcontentConstants.ADD_FILE_CLASSCONTENT_REQUEST_SENT });

        classcontentServices.sendAddFileRequest(auth, file)
            .then(response => dispatch({
                type: classcontentConstants.ADD_FILE_CLASSCONTENT_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: classcontentConstants.ADD_FILE_CLASSCONTENT_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeUpdateFileRequest = (auth: IAuthUser, file:IUpdateFile) => {
    return (dispatch: any) => {
        dispatch({ type: classcontentConstants.UPDATE_FILE_CLASSCONTENT_REQUEST_SENT });

        classcontentServices.sendUpdateFileRequest(auth, file)
            .then(response => dispatch({
                type: classcontentConstants.UPDATE_FILE_CLASSCONTENT_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: classcontentConstants.UPDATE_FILE_CLASSCONTENT_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeAddRichContentRequest = (auth: IAuthUser, richContent: IRichContent) => {
    return (dispatch: any) => {
        dispatch({ type: classcontentConstants.CREATE_RICHCONTENT_CLASSCONTENT_REQUEST_SENT});

        classcontentServices.sendRichContentRequest(auth,richContent)
            .then(response => dispatch({ 
                type: classcontentConstants.CREATE_RICHCONTENT_CLASSCONTENT_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({ 
                type: classcontentConstants.CREATE_RICHCONTENT_CLASSCONTENT_REQUEST_FAILED,
                error
        }))
    }
}

export const invokeImportRichContentRequest = (auth: IAuthUser, richContent: IRichContent) => {
    return (dispatch: any) => {
        dispatch({ type: classcontentConstants.IMPORT_RICHCONTENT_CLASSCONTENT_REQUEST_SENT});

        classcontentServices.sendUpdateRichContentRequest(auth,richContent)
            .then(response => dispatch({ 
                type: classcontentConstants.IMPORT_RICHCONTENT_CLASSCONTENT_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({ 
                type: classcontentConstants.IMPORT_RICHCONTENT_CLASSCONTENT_REQUEST_FAILED,
                error
        }))
    }
}

export const invokeUpdateRichContentRequest = (auth: IAuthUser, richContent: IRichContent) => {
    return (dispatch: any) => {
        dispatch({ type: classcontentConstants.UPDATE_RICHCONTENT_CLASSCONTENT_REQUEST_SENT});

        classcontentServices.sendUpdateRichContentRequest(auth,richContent)
            .then(response => dispatch({ 
                type: classcontentConstants.UPDATE_FILE_CLASSCONTENT_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({ 
                type: classcontentConstants.UPDATE_FILE_CLASSCONTENT_REQUEST_FAILED,
                error
        }))
    }
}

export const invokeGetClassContent = (auth: IAuthUser,  classroomId: string) => {
    return (dispatch: any) => {
        dispatch({ type: classcontentConstants.GET_CLASSCONTENT_REQUEST_SENT});

        classcontentServices.sendAllRequest(auth,classroomId)
            .then(response => dispatch({ 
                type: classcontentConstants.GET_CLASSCONTENT_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({ 
                type: classcontentConstants.GET_CLASSCONTENT_REQUEST_FAILED,
                error
        }))
    }
}

