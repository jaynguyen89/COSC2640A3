import React from 'react';
import {CLASSROOMS_PER_PAGE} from "../redux/interfaces";

const Pagination = (props: {
    paging: { offset: number, isLast: boolean },
    clickHandler: (next: boolean | null) => void
}) => {
    return (
        <>
            {
                props.paging.offset >= CLASSROOMS_PER_PAGE &&
                <>
                    <button className='btn waves-effect waves-light'
                            onClick={ () => props.clickHandler(null) }
                    >
                        <i className="fas fa-fast-backward" />&nbsp;
                        First Page
                    </button>

                    <button className='btn waves-effect waves-light'
                            style={{ marginLeft: '1em' }}
                            onClick={ () => props.clickHandler(false) }
                    >
                        <i className="fas fa-backward"/>&nbsp;
                        Previous Page
                    </button>
                </>
            }

            {
                !props.paging.isLast &&
                <button className='btn waves-effect waves-light'
                        style={{ marginLeft: '1em' }}
                        onClick={ () => props.clickHandler(true) }
                >
                    <i className="fas fa-forward"/>&nbsp;
                    Next Page
                </button>
            }
        </>
    );
}

export default Pagination;