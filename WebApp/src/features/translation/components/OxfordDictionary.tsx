import React from 'react';
import {ITranslation, ITranslationComponent} from "../redux/interfaces";

const OxfordDictionary = (props: ITranslationComponent) => {
    return (
        <>
            <div className='row' style={{ margin: 0 }}>
                <h6>Word: { (props.translation as ITranslation).word } - (root: { (props.translation as ITranslation).rootForm })</h6>
                <p>
                    <b>{ (props.translation as ITranslation).wordTypes.length === 1 ? 'Type' : 'Types' }:</b>&nbsp;
                    { (props.translation as ITranslation).wordTypes.join(', ') }
                </p>

                <p><b>Definitions:</b></p>
                {
                    Object.keys((props.translation as ITranslation).translations).map((type: string) =>
                        <p>
                            <b>- ({ type }):&nbsp;</b>
                            {
                                Object.values(
                                    (props.translation as ITranslation).translations
                                )[
                                    Object.keys((props.translation as ITranslation).translations).indexOf(type)
                                ].join(', ')
                            }
                        </p>
                    )
                }
            </div>

            <i className='fas fa-times text-link red-text'
               style={{ position: "absolute", width: '20px', height: '20px', top: 10, right: 10, textAlign: "right" }}
               onClick={ () => props.onClose() }
            />
        </>
    );
}

export default OxfordDictionary;