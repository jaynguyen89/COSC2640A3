import React from 'react';
import _ from 'lodash';
import {ITranslation, ITranslationComponent} from "../redux/interfaces";

const OxfordThesaurus = (props: ITranslationComponent) => {
    return (
        <>
            <div className='row' style={{ margin: 0 }}>
                <h6>Word: { (props.translation as ITranslation).word } - (root: { (props.translation as ITranslation).rootForm })</h6>
                <p>
                    <b>{ (props.translation as ITranslation).wordTypes.length === 1 ? 'Type' : 'Types' }:</b>&nbsp;
                    { (props.translation as ITranslation).wordTypes.join(', ') }
                </p>

                <p><b>{ _.isEmpty((props.translation as ITranslation).synonyms) ? 'Antonyms' : 'Synonyms' }:</b></p>
                {
                    Object.keys(
                        _.isEmpty((props.translation as ITranslation).synonyms)
                                ? (props.translation as ITranslation).antonyms
                                : (props.translation as ITranslation).synonyms
                    ).map((type: string) =>
                        <p>
                            <b>- ({ type }):&nbsp;</b>
                            {
                                Object.values(
                                    _.isEmpty((props.translation as ITranslation).synonyms)
                                            ? (props.translation as ITranslation).antonyms
                                            : (props.translation as ITranslation).synonyms
                                )[
                                    Object.keys(
                                        _.isEmpty((props.translation as ITranslation).synonyms)
                                                ? (props.translation as ITranslation).antonyms
                                                : (props.translation as ITranslation).synonyms
                                    ).indexOf(type)
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

export default OxfordThesaurus;