import React from 'react';
import {IFile} from "../redux/interfaces";
import {createFileUrl} from "../../../providers/helpers";

const PhotoSection = (props: { photos: Array<IFile>, classroomId: string }) => {
    return (
        <>
            {
                (
                    props.photos && props.photos.map(photo =>
                        <div key={ photo.id } className='col s3 center-align'>
                            <img src={ createFileUrl(photo, props.classroomId) } className='responsive-img' />
                            <span>{ photo.name }</span>
                        </div>
                    )
                ) ||
                <p>You haven't added any photo.</p>
            }
        </>
    );
}

export default PhotoSection;