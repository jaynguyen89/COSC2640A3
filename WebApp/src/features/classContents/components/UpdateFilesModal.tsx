import React from 'react';
import {IUpdateFilesModal} from "../redux/interfaces";
import {FileTypes} from "../../../providers/helpers";

const UpdateFilesModal = (props: IUpdateFilesModal) => {
    return (
        <div className='modal' id='updateFilesModal'>
            <div className='row'>
                <div className='col s12' style={{ marginBottom: '1em' }}>
                    <p className='section-header'>
                        <i className="fas fa-school" />&nbsp;&nbsp;
                        Update files
                    </p>
                    <span className='small-text'>You can add more files or remove files at the same time.</span>
                </div>

                <p className='col s12'>Select files to add more:</p>
                <div className='col s12'>
                    <div className="file-field input-field">
                        <div className="btn">
                            <span>Browse</span>
                            <input
                                accept={ props.fileType !== 3 ? `${ FileTypes[props.fileType] }/*` : `*/*` }
                                type="file"
                                multiple
                                onChange={ e => props.handleFilesInput(e.target.files as FileList) }
                            />
                        </div>
                        <div className="file-path-wrapper">
                            <input className="file-path validate" type="text" placeholder="Select files of the same type" />
                        </div>
                    </div>
                </div>

                <p className='col s12'>Select files to remove (optional):</p>
                <div className='col s12 center-align'>
                    {
                        props.files.map(file =>
                            <div key={ file.id } className='icon-list'>
                                <i className={
                                    file.type === 0 ? 'fas fa-file-video fa-3x' : (
                                        file.type === 1 ? 'fas fa-file-audio fa-3x' : (
                                            file.type === 2 ? 'fas fa-file-image fa-3x' : 'fas fa-file-invoice fa-3x'
                                        )
                                    )
                                } />
                                <p className='small-text'>{ file.name }</p>

                                {
                                    (
                                        props.selectedFiles.indexOf(file.id) === -1 &&
                                        <a className='text-link red-text'
                                           onClick={ () => props.removeBtnClick(file.id) }
                                        >
                                            Remove
                                        </a>
                                    ) ||
                                    <a className='text-link'
                                       onClick={ () => props.unselectBtnClick(file.id) }
                                    >
                                        Unselect
                                    </a>
                                }

                            </div>
                        )
                    }
                </div>

                <div className='col s12 center-align'>
                    <button className='btn waves-effect waves-light'
                            onClick={ () => props.updateBtnClick() }
                    >
                        <i className='fas fa-check' />&nbsp;
                        Update
                    </button>
                </div>
            </div>
        </div>
    );
}

export default UpdateFilesModal;