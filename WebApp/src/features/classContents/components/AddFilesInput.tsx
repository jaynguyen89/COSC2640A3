import React from 'react';
import {FileTypes} from "../../../providers/helpers";
import {IAddFilesInput} from "../redux/interfaces";

const AddFilesInput = (props: IAddFilesInput) => {
    return (
        <>
            <p className='title'>Select { (props.fileType === 3 ? 'file' : FileTypes[props.fileType]) }s to upload:</p>
            <div className="file-field input-field">
                <div className="btn">
                    <span>Browse</span>
                    <input
                        accept={ props.fileType !== 3 ? `${ FileTypes[props.fileType] }/*` : `*/*` }
                        type="file"
                        multiple
                        onChange={ e => props.handleFilesInput(e.target.files as FileList, props.fileType) }
                    />
                </div>
                <div className="file-path-wrapper">
                    <input className="file-path validate" type="text" placeholder="Select files of the same type" />
                </div>
            </div>

            <button className='btn waves-effect waves-light'
                    onClick={ () => props.confirmAdding() }
            >
                <i className="fas fa-upload" />&nbsp;
                Upload
            </button>
        </>
    );
}

export default AddFilesInput;