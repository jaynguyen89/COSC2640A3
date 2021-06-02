import * as translationConstants from './constants';
import produce from 'immer';
import {DEFAULT_ACTION_RESULT, IActionResult} from "../../../providers/helpers";

interface ITranslationStore {
    translateSentence: IActionResult,
    translateWord: IActionResult,
    getThesaurus: IActionResult
}

const initialState: ITranslationStore = {
    translateSentence: DEFAULT_ACTION_RESULT,
    translateWord: DEFAULT_ACTION_RESULT,
    getThesaurus: DEFAULT_ACTION_RESULT
}

const reducer = produce((state, action) => {
    switch (action.type) {
        case translationConstants.TRANSLATE_SENTENCE_REQUEST_SENT:
            state.translateSentence.action = translationConstants.TRANSLATE_SENTENCE_REQUEST_SENT;
            state.translateSentence.payload = null;
            state.translateSentence.error = null;
            return;
        case translationConstants.TRANSLATE_SENTENCE_REQUEST_SUCCESS:
            state.translateSentence.action = translationConstants.TRANSLATE_SENTENCE_REQUEST_SUCCESS;
            state.translateSentence.payload = action.payload;
            state.translateSentence.error = null;
            return;
        case translationConstants.TRANSLATE_SENTENCE_REQUEST_FAILED:
            state.translateSentence.action = translationConstants.TRANSLATE_SENTENCE_REQUEST_FAILED;
            state.translateSentence.payload = null;
            state.translateSentence.error = action.error;
            return;
        case translationConstants.TRANSLATE_WORD_REQUEST_SENT:
            state.translateWord.action = translationConstants.TRANSLATE_WORD_REQUEST_SENT;
            state.translateWord.payload = null;
            state.translateWord.error = null;
            return;
        case translationConstants.TRANSLATE_WORD_REQUEST_SUCCESS:
            state.translateWord.action = translationConstants.TRANSLATE_WORD_REQUEST_SUCCESS;
            state.translateWord.payload = action.payload;
            state.translateWord.error = null;
            return;
        case translationConstants.TRANSLATE_WORD_REQUEST_FAILED:
            state.translateWord.action = translationConstants.TRANSLATE_WORD_REQUEST_FAILED;
            state.translateWord.payload = null;
            state.translateWord.error = action.error;
            return;
        case translationConstants.GET_THESAURUS_REQUEST_SENT:
            state.getThesaurus.action = translationConstants.GET_THESAURUS_REQUEST_SENT;
            state.getThesaurus.payload = null;
            state.getThesaurus.error = null;
            return;
        case translationConstants.GET_THESAURUS_REQUEST_SUCCESS:
            state.getThesaurus.action = translationConstants.GET_THESAURUS_REQUEST_SUCCESS;
            state.getThesaurus.payload = action.payload;
            state.getThesaurus.error = null;
            return;
        case translationConstants.GET_THESAURUS_REQUEST_FAILED:
            state.getThesaurus.action = translationConstants.GET_THESAURUS_REQUEST_FAILED;
            state.getThesaurus.payload = null;
            state.getThesaurus.error = action.error;
            return;
        default:
            return;
    }
}, initialState);

export default reducer;