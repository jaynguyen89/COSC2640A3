import * as classcontentConstants from "./constants";
import produce from 'immer';
import {DEFAULT_ACTION_RESULT, IActionResult} from "../../../providers/helpers";

interface IClasscontnetStore {
    addFiles: IActionResult,
    updateFiles: IActionResult,
    addRichContent: IActionResult,
    importRichContent: IActionResult,
    updateRichContent: IActionResult,
    getClassContent: IActionResult
}

const initialState : IClasscontnetStore = {
    addFiles: DEFAULT_ACTION_RESULT,
    updateFiles: DEFAULT_ACTION_RESULT,
    addRichContent: DEFAULT_ACTION_RESULT,
    importRichContent: DEFAULT_ACTION_RESULT,
    updateRichContent: DEFAULT_ACTION_RESULT,
    getClassContent: DEFAULT_ACTION_RESULT
}

const reducer = produce((state, action) => {
    switch (action.type) {
        case classcontentConstants.ADD_FILE_CLASSCONTENT_REQUEST_SENT:
            state.addFileClassContent.action = classcontentConstants.ADD_FILE_CLASSCONTENT_REQUEST_SENT;
            state.addFileClassContent.payload = null;
            state.addFileClassContent.error = null;
            return;
        case classcontentConstants.ADD_FILE_CLASSCONTENT_REQUEST_SUCCESS:
            state.addFileClassContent.action = classcontentConstants.ADD_FILE_CLASSCONTENT_REQUEST_SUCCESS;
            state.addFileClassContent.payload = action.payload;
            state.addFileClassContent.error = null;
            return;
        case classcontentConstants.ADD_FILE_CLASSCONTENT_REQUEST_FAILED:
            state.addFileClassContent.action = classcontentConstants.ADD_FILE_CLASSCONTENT_REQUEST_FAILED;
            state.addFileClassContent.payload = null;
            state.addFileClassContent.error = action.error;
            return;
        case classcontentConstants.UPDATE_FILE_CLASSCONTENT_REQUEST_SENT:
            state.updateFileClassContent.action = classcontentConstants.UPDATE_FILE_CLASSCONTENT_REQUEST_SENT;
            state.updateFileClassContent.payload = null;
            state.updateFileClassContent.error = null;
            return;
        case classcontentConstants.UPDATE_FILE_CLASSCONTENT_REQUEST_SUCCESS:
            state.updateFileClassContent.action = classcontentConstants.UPDATE_FILE_CLASSCONTENT_REQUEST_SUCCESS;
            state.updateFileClassContent.payload = action.payload;
            state.updateFileClassContent.error = null;
            return;
        case classcontentConstants.UPDATE_FILE_CLASSCONTENT_REQUEST_FAILED:
            state.updateFileClassContent.action = classcontentConstants.UPDATE_FILE_CLASSCONTENT_REQUEST_FAILED;
            state.updateFileClassContent.payload = null;
            state.updateFileClassContent.error = action.error;
            return;
        case classcontentConstants.CREATE_RICHCONTENT_CLASSCONTENT_REQUEST_SENT:
            state.addRichContent.action = classcontentConstants.CREATE_RICHCONTENT_CLASSCONTENT_REQUEST_SENT;
            state.addRichContent.payload = null;
            state.addRichContent.error = null;
            return;
        case classcontentConstants.CREATE_RICHCONTENT_CLASSCONTENT_REQUEST_SUCCESS:
            state.addRichContent.action = classcontentConstants.CREATE_RICHCONTENT_CLASSCONTENT_REQUEST_SUCCESS;
            state.addRichContent.payload = action.payload;
            state.addRichContent.error = null;
            return;
        case classcontentConstants.CREATE_RICHCONTENT_CLASSCONTENT_REQUEST_FAILED:
            state.addRichContent.action = classcontentConstants.CREATE_RICHCONTENT_CLASSCONTENT_REQUEST_FAILED;
            state.addRichContent.payload = null;
            state.addRichContent.error = action.error;
            return;
        case classcontentConstants.UPDATE_RICHCONTENT_CLASSCONTENT_REQUEST_SENT:
            state.updateRichContent.action = classcontentConstants.UPDATE_RICHCONTENT_CLASSCONTENT_REQUEST_SENT;
            state.updateRichContent.payload = null;
            state.updateRichContent.error = null;
            return;
        case classcontentConstants.UPDATE_RICHCONTENT_CLASSCONTENT_REQUEST_SUCCESS:
            state.updateRichContent.action = classcontentConstants.UPDATE_RICHCONTENT_CLASSCONTENT_REQUEST_SUCCESS;
            state.updateRichContent.payload = action.payload;
            state.updateRichContent.error = null;
            return;
        case classcontentConstants.UPDATE_RICHCONTENT_CLASSCONTENT_REQUEST_FAILED:
            state.updateRichContent.action = classcontentConstants.UPDATE_RICHCONTENT_CLASSCONTENT_REQUEST_FAILED;
            state.updateRichContent.payload = null;
            state.updateRichContent.error = action.error;
            return;
        case classcontentConstants.IMPORT_RICHCONTENT_CLASSCONTENT_REQUEST_SENT:
            state.importRichContent.action = classcontentConstants.IMPORT_RICHCONTENT_CLASSCONTENT_REQUEST_SENT;
            state.importRichContent.payload = null;
            state.importRichContent.error = null;
            return;
        case classcontentConstants.IMPORT_RICHCONTENT_CLASSCONTENT_REQUEST_SUCCESS:
            state.importRichContent.action = classcontentConstants.IMPORT_RICHCONTENT_CLASSCONTENT_REQUEST_SUCCESS;
            state.importRichContent.payload = action.payload;
            state.importRichContent.error = null;
            return;
        case classcontentConstants.IMPORT_RICHCONTENT_CLASSCONTENT_REQUEST_FAILED:
            state.importRichContent.action = classcontentConstants.IMPORT_RICHCONTENT_CLASSCONTENT_REQUEST_FAILED;
            state.importRichContent.payload = null;
            state.importRichContent.error = action.error;
            return;
        case classcontentConstants.GET_CLASSCONTENT_REQUEST_SENT:
            state.getClassContent.action = classcontentConstants.GET_CLASSCONTENT_REQUEST_SENT;
            state.getClassContent.payload = null;
            state.getClassContent.error = null;
            return;
        case classcontentConstants.GET_CLASSCONTENT_REQUEST_SUCCESS:
            state.getClassContent.action = classcontentConstants.GET_CLASSCONTENT_REQUEST_SUCCESS;
            state.getClassContent.payload = action.payload;
            state.getClassContent.error = null;
            return;
        case classcontentConstants.GET_CLASSCONTENT_REQUEST_FAILED:
            state.getClassContent.action = classcontentConstants.GET_CLASSCONTENT_REQUEST_FAILED;
            state.getClassContent.payload = null;
            state.getClassContent.error = action.error;
            return;
        default:
            return;
    }
}, initialState);

export default reducer;