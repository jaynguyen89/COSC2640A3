import React from 'react';

const Popover = (props: { children: JSX.Element }) => {
    return (
        <div className='popover'>{ props.children }</div>
    );
}

export default Popover;