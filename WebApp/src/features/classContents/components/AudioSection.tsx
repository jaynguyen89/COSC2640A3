import React from "react";
import {IFile} from "../redux/interfaces";
import {createFileUrl} from "../../../providers/helpers";

const AudioSection = (props: { audios: Array<IFile>, classroomId: string }) => {
    return (
        <>
            {
                (
                    props.audios.length !== 0 && props.audios.map(audio =>
                        <div className='row' key={ audio.id } >
                            <div className='col s12'>
                                <audio controls style={{ width: '100%' }}>
                                    <source src={ createFileUrl(audio, props.classroomId) } type={ `audio/mpeg` } />
                                </audio>
                                <p className='small-text center-align' style={{ margin: 0 }}>{ audio.name }</p>
                            </div>
                        </div>
                    )
                ) ||
                <p style={{ marginLeft: '1em' }}>
                    <i className="far fa-meh" />&nbsp;
                    No audio has been added.
                </p>
            }
        </>
    );
}

export default AudioSection;