import * as contentConstants from './constants';
import produce from 'immer';
import {DEFAULT_ACTION_RESULT, IActionResult} from "../../../providers/helpers";

interface IContentStore {
    getContent: IActionResult,
    addFiles: IActionResult,
    updateFiles: IActionResult,
    addRichContent: IActionResult,
    importRichContent: IActionResult,
    updateRichContent: IActionResult
}

const initialState: IContentStore = {
    getContent: DEFAULT_ACTION_RESULT,
    addFiles: DEFAULT_ACTION_RESULT,
    updateFiles: DEFAULT_ACTION_RESULT,
    addRichContent: DEFAULT_ACTION_RESULT,
    importRichContent: DEFAULT_ACTION_RESULT,
    updateRichContent: DEFAULT_ACTION_RESULT
}

const reducer = produce((state, action) => {
    switch (action.type) {
        case contentConstants.GET_CLASSROOM_CONTENTS_REQUEST_SENT:
            state.getContent.action = contentConstants.GET_CLASSROOM_CONTENTS_REQUEST_SENT;
            state.getContent.payload = null;
            state.getContent.error = null;
            return;
        case contentConstants.GET_CLASSROOM_CONTENTS_REQUEST_SUCCESS:
            state.getContent.action = contentConstants.GET_CLASSROOM_CONTENTS_REQUEST_SUCCESS;
            state.getContent.payload = action.payload;
            state.getContent.error = null;
            return;
        case contentConstants.GET_CLASSROOM_CONTENTS_REQUEST_FAILED:
            state.getContent.action = contentConstants.GET_CLASSROOM_CONTENTS_REQUEST_FAILED;
            state.getContent.payload = null;
            state.getContent.error = action.error;
            return;
        case contentConstants.ADD_FILES_TO_CLASSROOM_REQUEST_SENT:
            state.addFiles.action = contentConstants.ADD_FILES_TO_CLASSROOM_REQUEST_SENT;
            state.addFiles.payload = null;
            state.addFiles.error = null;
            return;
        case contentConstants.ADD_FILES_TO_CLASSROOM_REQUEST_SUCCESS:
            state.addFiles.action = contentConstants.ADD_FILES_TO_CLASSROOM_REQUEST_SUCCESS;
            state.addFiles.payload = action.payload;
            state.addFiles.error = null;
            return;
        case contentConstants.ADD_FILES_TO_CLASSROOM_REQUEST_FAILED:
            state.addFiles.action = contentConstants.ADD_FILES_TO_CLASSROOM_REQUEST_FAILED;
            state.addFiles.payload = null;
            state.addFiles.error = action.error;
            return;
        case contentConstants.UPDATE_OR_REMOVE_FILES_REQUEST_SENT:
            state.updateFiles.action = contentConstants.UPDATE_OR_REMOVE_FILES_REQUEST_SENT;
            state.updateFiles.payload = null;
            state.updateFiles.error = null;
            return;
        case contentConstants.UPDATE_OR_REMOVE_FILES_REQUEST_SUCCESS:
            state.updateFiles.action = contentConstants.UPDATE_OR_REMOVE_FILES_REQUEST_SUCCESS;
            state.updateFiles.payload = action.payload;
            state.updateFiles.error = null;
            return;
        case contentConstants.UPDATE_OR_REMOVE_FILES_REQUEST_FAILED:
            state.updateFiles.action = contentConstants.UPDATE_OR_REMOVE_FILES_REQUEST_FAILED;
            state.updateFiles.payload = null;
            state.updateFiles.error = action.error;
            return;
        case contentConstants.ADD_RICH_CONTENT_REQUEST_SENT:
            state.addRichContent.action = contentConstants.ADD_RICH_CONTENT_REQUEST_SENT;
            state.addRichContent.payload = null;
            state.addRichContent.error = null;
            return;
        case contentConstants.ADD_RICH_CONTENT_REQUEST_SUCCESS:
            state.addRichContent.action = contentConstants.ADD_RICH_CONTENT_REQUEST_SUCCESS;
            state.addRichContent.payload = action.payload;
            state.addRichContent.error = null;
            return;
        case contentConstants.ADD_RICH_CONTENT_REQUEST_FAILED:
            state.addRichContent.action = contentConstants.ADD_RICH_CONTENT_REQUEST_FAILED;
            state.addRichContent.payload = null;
            state.addRichContent.error = action.error;
            return;
        case contentConstants.IMPORT_RICH_CONTENT_REQUEST_SENT:
            state.importRichContent.action = contentConstants.IMPORT_RICH_CONTENT_REQUEST_SENT;
            state.importRichContent.payload = null;
            state.importRichContent.error = null;
            return;
        case contentConstants.IMPORT_RICH_CONTENT_REQUEST_SUCCESS:
            state.importRichContent.action = contentConstants.IMPORT_RICH_CONTENT_REQUEST_SUCCESS;
            state.importRichContent.payload = action.payload;
            state.importRichContent.error = null;
            return;
        case contentConstants.IMPORT_RICH_CONTENT_REQUEST_FAILED:
            state.importRichContent.action = contentConstants.IMPORT_RICH_CONTENT_REQUEST_FAILED;
            state.importRichContent.payload = null;
            state.importRichContent.error = action.error;
            return;
        case contentConstants.UPDATE_RICH_CONTENT_REQUEST_SENT:
            state.updateRichContent.action = contentConstants.UPDATE_RICH_CONTENT_REQUEST_SENT;
            state.updateRichContent.payload = null;
            state.updateRichContent.error = null;
            return;
        case contentConstants.UPDATE_RICH_CONTENT_REQUEST_SUCCESS:
            state.updateRichContent.action = contentConstants.UPDATE_RICH_CONTENT_REQUEST_SUCCESS;
            state.updateRichContent.payload = action.payload;
            state.updateRichContent.error = null;
            return;
        case contentConstants.UPDATE_RICH_CONTENT_REQUEST_FAILED:
            state.updateRichContent.action = contentConstants.UPDATE_RICH_CONTENT_REQUEST_FAILED;
            state.updateRichContent.payload = null;
            state.updateRichContent.error = action.error;
            return;
        default:
            return;

    }
}, initialState);

export default reducer;