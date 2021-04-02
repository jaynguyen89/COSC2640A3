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
            <div className={ 'card ' + (ALERT_CLASS as any)[props.type] }>
                <div className='card-content'>
                    <span className='card-title'>{ props.type === 'error' ? 'Error!' : 'Success' }</span>
                    {
                        props.messages.map((message: string, i: number) =>
                            <p key={ i }>{ message }</p>
                        )
                    }
                </div>
            </div>
        ) || <></>
    );
}

export default Alert;