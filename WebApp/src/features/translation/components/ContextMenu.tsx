import React from 'react';
import {connect} from "react-redux";
import {IContextMenu, MenuType} from "../redux/interfaces";
import {Languages} from "../../../providers/helpers";
import Spinner from "../../../shared/Spinner";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser
});

const mapActionsToProps = {};

const ContextMenu = (props: IContextMenu) => {
    const [shouldHaltView, setShouldHaltView] = React.useState(false);
    const [targetLanguage, setTargetLanguage] = React.useState(1);

    const handleButtons = (name: string) => {
        if (name === 'translate') props.onTranslate(props.text, targetLanguage);
        if (name === 'define') props.onDefine(props.text, targetLanguage);
        if (name === 'synonyms') props.onThesaurus(props.text, true);
        if (name === 'antonyms') props.onThesaurus(props.text, false);
        setShouldHaltView(true);
    }

    return (
        <div className='row' style={{ margin: 0 }}>
            { shouldHaltView && <Spinner /> }

            <p className='col s12' style={{ textOverflow: 'ellipsis', whiteSpace: 'nowrap', width: '100%', overflow: 'hidden', marginBottom: '0.5em' }}>
                {
                    (
                        props.menuType === MenuType.Translation &&
                        <b>Text:</b>
                    ) || <b>Word:</b>
                }
                &nbsp;{ props.text }
            </p>

            <div className="input-field col s12" style={{ margin: 0, padding: 0 }}>
                <select className='browser-default' value={ targetLanguage }
                        onChange={ e => setTargetLanguage(e.target.selectedIndex + 1) }
                >
                    {
                        Languages.map(language =>
                            <option key={ language.index } value={ language.index }>{ language.text }</option>
                        )
                    }
                </select>
            </div>

            <div className='col s12' style={{ padding: 0, marginTop: '5px' }}>
                {
                    (
                        props.menuType === MenuType.Translation &&
                        <button className={ (shouldHaltView && 'btn btn-mini disabled') || 'btn btn-mini waves-effect waves-light' }
                                onClick={ () => handleButtons('translate') }
                        >
                            Translate
                        </button>
                    ) ||
                    <>
                        <button className={ (shouldHaltView && 'btn btn-mini disabled') || 'btn btn-mini waves-effect waves-light' }
                                onClick={ () => handleButtons('define') }
                        >
                            Define
                        </button>

                        <button className={ (shouldHaltView && 'btn btn-mini disabled') || 'btn btn-mini waves-effect waves-light' }
                                style={{ marginLeft: '1em' }}
                                onClick={ () => handleButtons('synonyms') }
                        >
                            Synonyms
                        </button>

                        <button className={ (shouldHaltView && 'btn btn-mini disabled') || 'btn btn-mini waves-effect waves-light' }
                                style={{ marginLeft: '1em' }}
                                onClick={ () => handleButtons('antonyms') }
                        >
                            Antonyms
                        </button>
                    </>
                }

                <a className='text-link amber-text right' onClick={ () => { setShouldHaltView(false); props.onClose(); }}>Close</a>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ContextMenu);