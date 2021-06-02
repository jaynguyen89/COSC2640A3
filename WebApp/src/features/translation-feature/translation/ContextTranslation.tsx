import React from 'react';
import {connect} from "react-redux";
import {IContextTranslation, ITranslateIt, ITranslation, MenuType} from "./redux/interfaces";
import ContextMenu from "./components/ContextMenu";
import Popover from "../../shared/Popover";
import $ from 'jquery';
import {invokeGetThesaurusRequest, invokeTranslateSentenceRequest, invokeTranslateWordRequest} from "./redux/actions";
import * as translationConstants from './redux/constants';
import {checkSession, removeGlobalMessage, setGlobalMessage} from "../../providers/helpers";
import {clearAuthUser} from "../authentication/redux/actions";
import AwsTranslation from "./components/AwsTranslation";
import OxfordDictionary from "./components/OxfordDictionary";
import OxfordThesaurus from "./components/OxfordThesaurus";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    translateSentence: state.translationStore.translateSentence,
    translateWord: state.translationStore.translateWord,
    getThesaurus: state.translationStore.getThesaurus
});

const mapActionsToProps = {
    clearAuthUser,
    invokeTranslateSentenceRequest,
    invokeTranslateWordRequest,
    invokeGetThesaurusRequest
};

const ContextTranslation = (props: IContextTranslation) => {
    const [shouldShowPopover, setShouldShowPopover] = React.useState(false);
    const [coordination, setCoordination] = React.useState({ x: 0, y: 0 });
    const [popoverContent, setPopoverContent] = React.useState(null as unknown as JSX.Element);

    const handleRightClicked = (e: any) => {
        const selectedText = window.getSelection()?.toString() as string;
        if (selectedText === null || selectedText === undefined || selectedText.length === 0) return;
        e.preventDefault();

        setPopoverContent(
            <ContextMenu
                menuType={ selectedText.trim().split(' ').length === 1 ? MenuType.Dictionary : MenuType.Translation }
                text={ selectedText.trim() }
                onTranslate={ handleTranslateBtnClicked }
                onDefine={ handleDefineBtnClicked }
                onThesaurus={ handleThesaurusBtnClicked }
                onClose={ handleClosePopover }
            />
        );

        setShouldShowPopover(true);
        setCoordination({ x: e.clientX, y: e.clientY });
    }

    const handleClosePopover = () => {
        setShouldShowPopover(false);
        setCoordination({ x: 0, y: 0 });
    }

    React.useEffect(() => {
        const popover = $('.popover');

        if (shouldShowPopover) {
            popover.css({ top: `${ coordination.y + 10 }px`, left: `${ coordination.x + 10 }px` });
            popover.css('display', 'block');
            return;
        }

        popover.css('display', 'none');
        setPopoverContent(null as unknown as JSX.Element);
    }, [shouldShowPopover]);

    const handleTranslateBtnClicked = (phrase: string, target: number) => {
        const translationData = {
            phrase: phrase,
            targetLanguage: target,
            forSynonyms: false
        } as ITranslateIt;
        props.invokeTranslateSentenceRequest(props.authUser, translationData);
    }

    const handleDefineBtnClicked = (word: string, target: number) => {
        const translationData = {
            phrase: word,
            targetLanguage: target,
            forSynonyms: false
        } as ITranslateIt;
        props.invokeTranslateWordRequest(props.authUser, translationData);
    }

    const handleThesaurusBtnClicked = (word: string, forSynonyms: boolean) => {
        const translationData = {
            phrase: word,
            targetLanguage: 0,
            forSynonyms: forSynonyms
        } as ITranslateIt;
        props.invokeGetThesaurusRequest(props.authUser, translationData);
    }

    React.useEffect(() => {
        if (props.translateSentence.action === translationConstants.TRANSLATE_SENTENCE_REQUEST_FAILED) {
            const result = checkSession(props.clearAuthUser, setGlobalMessage, props.translateSentence.error?.message);
            if (result) {
                removeGlobalMessage();
                alert('An issue happened while processing your request. Please try again.');
            }
        }

        if (props.translateSentence.action === translationConstants.TRANSLATE_SENTENCE_REQUEST_SUCCESS)
            if (props.translateSentence.payload === null)
                alert('Error! Failed to send request to server. Please try again.');
            else if (props.translateSentence.payload.result === 0)
                alert('Error! ' + props.translateSentence.payload.messages.join());
            else
                setPopoverContent(
                    <AwsTranslation
                        translation={props.translateSentence.payload.data as unknown as string }
                        onClose={ handleClosePopover }
                    />
                );
    }, [props.translateSentence]);

    React.useEffect(() => {
        if (props.translateWord.action === translationConstants.TRANSLATE_WORD_REQUEST_FAILED) {
            const result = checkSession(props.clearAuthUser, setGlobalMessage, props.translateWord.error?.message);
            if (result) {
                removeGlobalMessage();
                alert('An issue happened while processing your request. Please try again.');
            }
        }

        if (props.translateWord.action === translationConstants.TRANSLATE_WORD_REQUEST_SUCCESS)
            if (props.translateWord.payload === null)
                alert('Error! Failed to send request to server. Please try again.');
            else if (props.translateWord.payload.result === 0)
                alert('Unable to find a word that match your selected word. Please try again.');
            else
                setPopoverContent(
                    <OxfordDictionary
                        translation={ props.translateWord.payload.data as ITranslation }
                        onClose={ handleClosePopover }
                    />
                );
    }, [props.translateWord]);

    React.useEffect(() => {
        if (props.getThesaurus.action === translationConstants.GET_THESAURUS_REQUEST_FAILED) {
            const result = checkSession(props.clearAuthUser, setGlobalMessage, props.getThesaurus.error?.message);
            if (result) {
                removeGlobalMessage();
                alert('An issue happened while processing your request. Please try again.');
            }
        }

        if (props.getThesaurus.action === translationConstants.GET_THESAURUS_REQUEST_SUCCESS)
            if (props.getThesaurus.payload === null)
                alert('Error! Failed to send request to server. Please try again.');
            else if (props.getThesaurus.payload.result === 0)
                alert('Unable to find a word that match your selected word. Please try again.');
            else
                setPopoverContent(
                    <OxfordThesaurus translation={ props.getThesaurus.payload.data as ITranslation }
                                     onClose={ handleClosePopover }
                    />
                );
    }, [props.getThesaurus]);

    return (
        <div onContextMenu={ e => handleRightClicked(e) }>
            {
                shouldShowPopover &&
                <Popover>{ popoverContent }</Popover>
            }

            { props.children }
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ContextTranslation);