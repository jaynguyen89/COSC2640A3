import React from 'react';

const Spinner = ({ size } : { size?: string }) => {
    return (
        <div className='center-align'>
            <div className={ size === undefined ? 'preloader-wrapper active' : 'spinner-wrapper active ' + size }>
                <div className='spinner-layer spinner-teal-only'>
                    <div className='circle-clipper left'>
                        <div className='circle'></div>
                    </div>
                    <div className='gap-patch'>
                        <div className='circle'></div>
                    </div>
                    <div className='circle-clipper right'>
                        <div className='circle'></div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Spinner;