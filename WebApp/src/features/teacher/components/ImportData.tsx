import React from 'react';
import Spinner from "../../../shared/Spinner";
import Alert from "../../../shared/Alert";
import {EMPTY_STRING} from "../../../providers/helpers";
import {IImportData} from "../redux/interfaces";

const ImportData = (props: IImportData) => {
    return (
        <div className='row'>
            <div className='col m6 s12'>
                <p className='title section-header'>
                    <i className="fas fa-angle-right" />
                    &nbsp;Import classrooms
                </p>

                <div className="file-field input-field">
                    <div className="btn">
                        <span><i className="fas fa-file-alt" />&nbsp;Select File</span>
                        <input type="file"
                               disabled={ !props.enableButtons }
                               onChange={ e => props.handleSelection(e.target.files, 'classrooms') }
                        />
                    </div>
                    <div className="file-path-wrapper">
                        <input className="file-path validate"
                               type="text"
                               readOnly
                               value={ (props.selectedFile.importType === 0 && props.selectedFile.fileForImport && props.selectedFile.fileForImport.name) || EMPTY_STRING }
                               placeholder='Select 1 JSON file'
                        />
                    </div>
                    <p className='small-text'>Instead of creating each classroom, you can import a JSON file to create multiple classrooms at once.</p>
                </div>
            </div>

            <div className='col m6 s12'>
                <p className='title section-header'>
                    <i className="fas fa-angle-right" />
                    &nbsp;Import students
                </p>

                <div className="file-field input-field">
                    <div className="btn">
                        <span><i className="fas fa-file-alt" />&nbsp;Select File</span>
                        <input type="file"
                               disabled={ !props.enableButtons }
                               onChange={ e => props.handleSelection(e.target.files, 'students') }
                        />
                    </div>
                    <div className="file-path-wrapper">
                        <input className="file-path validate"
                               type="text"
                               readOnly
                               value={ (props.selectedFile.importType === 1 && props.selectedFile.fileForImport && props.selectedFile.fileForImport.name) || EMPTY_STRING }
                               placeholder='Select 1 JSON file'
                        />
                    </div>
                    <p className='small-text'>Students will be imported into classrooms along with their enrolment data without you having to add them one-by-one.</p>
                </div>
            </div>

            <div className='col s12 center-align'>
                { props.shouldShowSpinner && <Spinner /> }
                <Alert { ...props.statusMessage } closeAlert={ props.closeStatusMessage } />

                <button className={ (props.enableButtons && 'btn waves-effect waves-light') || 'btn disabled' }
                        onClick={ () => props.attemptUploadingFileForImport() }
                >
                    {
                        (props.selectedFile.fileForImport === null && 'Select a file from either side above first') || (
                            (props.selectedFile.importType === 0 && 'Upload Classroom Import File') || 'Upload Student Import File'
                        )
                    }
                </button>
            </div>
        </div>
    );
}

export default ImportData;