import React from 'react';
import {IFile} from "../redux/interfaces";

const VideoSection = (props: { videos: Array<IFile>, selectedVideo: IFile, classroomId: string, onSelectVideo: (videoId: string) => void }) => {
    return (
        <>
            {
                (
                    props.videos.length !== 0 && props.videos.map(video =>
                        <div className={ (props.selectedVideo.id === video.id && 'horizontal-item horizontal-item-active') || 'horizontal-item' }
                             key={ video.id } onClick={ () => props.onSelectVideo(video.id) }
                        >
                            <i className="fas fa-video fa-3x" />
                            <p className='small-text' style={{ margin: 0 }}>
                                { video.name }
                            </p>
                        </div>
                    )
                ) ||
                <p style={{ marginLeft: '1em' }}>
                    <i className="far fa-meh" />&nbsp;
                    No video has been added.
                </p>
            }
        </>
    );
}

export default VideoSection;