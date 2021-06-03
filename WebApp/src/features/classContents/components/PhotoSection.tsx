import React from 'react';
import {IFile} from "../redux/interfaces";
import {createFileUrl} from "../../../providers/helpers";

const PhotoSection = (props: { photos: Array<IFile>, classroomId: string }) => {
    return (
        <>
            {
                (
                    props.photos.length !== 0 && props.photos.map(photo =>
                        <div key={ photo.id } className='col s3 center-align'>
                            <img src={ createFileUrl(photo, props.classroomId) } className='responsive-img' />
                            <span>{ photo.name }</span>
                        </div>
                    )
                ) ||
                <p style={{ marginLeft: '1em' }}>
                    <i className="far fa-meh" />&nbsp;
                    No photo has been added.
                </p>
            }
        </>
    );
}

export default PhotoSection;