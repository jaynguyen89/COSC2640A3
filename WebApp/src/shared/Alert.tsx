import React from 'react';
import { IStatusMessage } from "../providers/helpers";

const ALERT_CLASS = {
    'error': 'alert-error',
    'success': 'alert-success'
};

const Alert = (props: IStatusMessage) => {
    return (
        (
            props.messages && props.messages.length !== 0 &&
            <div className={ 'alert ' + (ALERT_CLASS as any)[props.type] }>
                <span className="close-btn" onClick={ props.closeAlert }>&times;</span>
                <span className='title'>{ props.type === 'error' ? 'Error!' : 'Success!' }</span>
                {
                    (
                        props.messages.length === 1 &&
                        <span>&nbsp;{ props.messages[0] }</span>
                    ) ||
                    props.messages.map((message: string, i: number) =>
                        <p key={ i }>{ message }</p>
                    )
                }
            </div>
        ) || <></>
    );
}

export default Alert;