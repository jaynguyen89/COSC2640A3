import React from 'react';
import {IFile} from "../redux/interfaces";
import {createFileUrl} from "../../../providers/helpers";

const AttachmentSection = (props: { attachments: Array<IFile>, classroomId: string }) => {
    return (
        <>
            {
                (
                    props.attachments.length !== 0 && props.attachments.map(attachment =>
                        <div key={ attachment.id }
                             className='col m6 s2 center-align attachment'
                             style={{ marginBottom: '0.5em' }}
                             onClick={ () => {
                                 const downloadLink = document.createElement('a');
                                 downloadLink.href = createFileUrl(attachment, props.classroomId);
                                 downloadLink.setAttribute('download', attachment.name);

                                 document.body.appendChild(downloadLink);
                                 downloadLink.click();
                                 document.body.removeChild(downloadLink);
                             }}
                        >
                            <i className="fas fa-file fa-3x" />
                            <p className='small-text'>{ attachment.name }</p>
                        </div>
                    )
                ) ||
                <p style={{ marginLeft: '1em' }}>
                    <i className="far fa-meh" />&nbsp;
                    No attachment has been added.
                </p>
            }
        </>
    );
}

export default AttachmentSection;