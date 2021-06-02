import {IAuthUser} from "../../authentication/redux/interfaces";
import {IActionResult} from "../../../providers/helpers";

export interface IContextTranslation {
    authUser: IAuthUser,
    children: JSX.Element,
    clearAuthUser: () => void,
    translateSentence: IActionResult,
    translateWord: IActionResult,
    getThesaurus: IActionResult,
    invokeTranslateSentenceRequest: (auth: IAuthUser, data: ITranslateIt) => void,
    invokeTranslateWordRequest: (auth: IAuthUser, data: ITranslateIt) => void,
    invokeGetThesaurusRequest: (auth: IAuthUser, data: ITranslateIt) => void
}

export enum MenuType {
    Dictionary,
    Translation
}

export interface IContextMenu {
    authUser: IAuthUser,
    menuType: MenuType,
    text: string,
    onClose: () => void,
    onTranslate: (phrase: string, target: number) => void,
    onDefine: (word: string, target: number) => void,
    onThesaurus: (word: string, forSynonyms: boolean) => void
}

export interface ITranslateIt {
    targetLanguage: number,
    phrase: string,
    forSynonyms: boolean
}

export interface ITranslation {
    word: string,
    rootForm: string,
    wordTypes: Array<string>,
    translations: Object,
    synonyms: Object,
    antonyms: Object
}

export interface ITranslationComponent {
    translation: ITranslation | string,
    onClose: () => void
}