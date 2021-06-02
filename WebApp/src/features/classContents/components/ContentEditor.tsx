import React from 'react';
import {EditorState, ContentState} from 'draft-js';
import { Editor } from "react-draft-wysiwyg";
import "react-draft-wysiwyg/dist/react-draft-wysiwyg.css";
import {IContentEditor} from "../redux/interfaces";
import { convertFromHTML } from "draft-js";
import { stateToHTML } from 'draft-js-export-html';

const ContentEditor = (props: IContentEditor) => {
    const [editorState, setEditorState] = React.useState(() =>
        EditorState.createWithContent(
            ContentState.createFromBlockArray(convertFromHTML(props.content).contentBlocks)
        )
    );

    return (
        <Editor
            wrapperClassName="rte-wrapper"
            editorClassName="rte-editor"
            placeholder="Enter main contents of this classroom"
            editorState={ editorState }
            onEditorStateChange={ (state) => {
                setEditorState(state);
                props.informChanges(stateToHTML(editorState.getCurrentContent()));
            }}
        />
    );
}

export default ContentEditor;