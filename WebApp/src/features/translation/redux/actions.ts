import * as translationConstants from './constants';
import * as translationServices from './services';
import {IAuthUser} from "../../authentication/redux/interfaces";
import {ITranslateIt} from "./interfaces";

export const invokeTranslateSentenceRequest = (auth: IAuthUser, data: ITranslateIt) => {
    return (dispatch: any) => {
        dispatch({ type: translationConstants.TRANSLATE_SENTENCE_REQUEST_SENT });

        translationServices.sendTranslateSentenceRequest(auth, data)
            .then(response => dispatch({
                type: translationConstants.TRANSLATE_SENTENCE_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: translationConstants.TRANSLATE_SENTENCE_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeTranslateWordRequest = (auth: IAuthUser, data: ITranslateIt) => {
    return (dispatch: any) => {
        dispatch({ type: translationConstants.TRANSLATE_WORD_REQUEST_SENT });

        translationServices.sendTranslateWordRequest(auth, data)
            .then(response => dispatch({
                type: translationConstants.TRANSLATE_WORD_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: translationConstants.TRANSLATE_WORD_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeGetThesaurusRequest = (auth: IAuthUser, data: ITranslateIt) => {
    return (dispatch: any) => {
        dispatch({ type: translationConstants.GET_THESAURUS_REQUEST_SENT });

        translationServices.sendGetThesaurusRequest(auth, data)
            .then(response => dispatch({
                type: translationConstants.GET_THESAURUS_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: translationConstants.GET_THESAURUS_REQUEST_FAILED,
                error
            }))
    };
}