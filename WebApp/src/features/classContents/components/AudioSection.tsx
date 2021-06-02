import React from "react";
import {IFile} from "../redux/interfaces";
import {createFileUrl} from "../../../providers/helpers";

const AudioSection = (props: { audios: Array<IFile>, classroomId: string }) => {
    return (
        <>
            {
                (
                    props.audios && props.audios.map(audio =>
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
                <div className='card horizontal center'>
                    <p>You haven't added any audio file.</p>
                </div>
            }
        </>
    );
}

export default AudioSection;