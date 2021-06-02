import React from 'react';
import {ITranslationComponent} from "../redux/interfaces";

const AwsTranslation = (props: ITranslationComponent) => {
    return (
        <>
            <div className='row' style={{ margin: 0 }}>
                <h6>Translated text:</h6>
                <p>{ props.translation as string }</p>
            </div>

            <i className='fas fa-times text-link red-text'
               style={{ position: "absolute", width: '20px', height: '20px', top: 10, right: 10, textAlign: "right" }}
               onClick={ () => props.onClose() }
            />
        </>
    );
}

export default AwsTranslation;