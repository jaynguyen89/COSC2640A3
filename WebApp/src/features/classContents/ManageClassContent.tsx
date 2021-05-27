import React from 'react';
import { connect } from 'react-redux';
import _ from 'lodash';
import $ from 'jquery';
import M from 'materialize-css';
import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import { clearAuthUser } from "../authentication/redux/actions";
import { IClassContent, IFile, IManageClassroomContent, defaultRichContent, IUploadFile, FileType, IFileAdding} from "./redux/interfaces";
import {
    invokeAddFileRequest, invokeUpdateFileRequest,
    invokeAddRichContentRequest, invokeImportRichContentRequest,
    invokeUpdateRichContentRequest, invokeGetClassContent
} from "./redux/actions";
import {
    checkSession,
    EMPTY_STATUS,
    EMPTY_STRING,
    IStatusMessage,
    modalOptions, removeGlobalMessage, setGlobalMessage, TASK_CREATE, TASK_UPDATE,
    TASK_VIEW
} from "../../providers/helpers";
import moment from "moment";
import FileList from '../classContents/components/FileList';
import HeaderNav from "../../shared/HeaderNav";
import { Editor, EditorState } from "react-draft-wysiwyg";
import "react-draft-wysiwyg/dist/react-draft-wysiwyg.css";
import * as classContentConstants from "./redux/constants";



const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    addFileClassContent: state.classcontentStore.addFileClassContent,
    updateFileClassContent: state.classcontentStore.updateFileClassContent,
    addRichContent: state.classcontentStore.addRichContent,
    importRichContent: state.classcontentStore.importRichContent,
    updateRichContent: state.classcontentStore.updateRichContent,
    getclassContent: state.classcontentStore.getclassContent
});

const mapActionsToProps = {
    clearAuthUser,
    invokeAddFileRequest,
    invokeUpdateFileRequest,
    invokeAddRichContentRequest,
    invokeImportRichContentRequest,
    invokeUpdateRichContentRequest,
    invokeGetClassContent,
};


const ManageClassContent = (props: IManageClassroomContent) => {
    const [addfiles, setAddFiles] = React.useState(Array<IFile>());
    const [Updatefiles, setUpdateFiles] = React.useState(Array<IFile>());
    const [addRichContent, setAddRichContent] = React.useState(defaultRichContent); 

//Write a const that handles the upload button function. 

    const [fileType, setFileType] = React.useState(FileType.photo)

    

    // const UploadFileForImport = () => {
    //     if (addfiles.uploadedFiles === null) {
    //         alert('You have selected no file to upload.');
    //         return;
    //     }

    //     setFileFieldsEnabled(false);
    //     props.invokeUploadFileForImportRequest(props.authUser, selectedFile);
    // }

    // React.useEffect(() => {
    //     if (props.AddFileRequest.action === classContentConstants.ADD_FILE_CLASSCONTENT_REQUEST_FAILED)
    //         checkSession(props.clearAuthUser, props.AddFileRequest.error?.message);

     
    //     setFileFieldsEnabled(true);
    // }, [props.AddFileRequest]);

    const attemptUploadingFiles = () => {
        props.invokeAddFileRequest (props.authUser, {classroomID:props.classroomId , fileType:FileType.photo, uploadedFiles:addfiles} as IFileAdding )
    }

    return (
        <div className='container' style={{ marginTop: '2.5em' }}>
        <HeaderNav
            location='manage-classrooms'
            greetingName={ localStorage.getItem('preferredName') as string }
        />
           <div className='card'>
                <div className='card-content'>
                    <div className='row'>
                        <div className='rol s12'>
                            <p className='card-title left'>
                                &nbsp; Add your files here:
                            </p>
                        </div>
                     </div>
                    <div className='row'>   
                    <div className="file-field input-field">
                    <div className="btn">
                        <span><i className="fas fa-file-alt" />&nbsp;Select File</span>
                        <input type="file"
                              
                        />
                    </div>
                    <div className="file-path-wrapper">
                        <input className="file-path validate"
                               type="file"
                               multiple
                               //TODO: Add the new file types to this
                               //accept={(fileType === FileType.photo && ".jpg, .jpeg, .png, .bmp, .tiff, .gif") || fileType === FileType.audio && ".mp3, .wav" || fileType === FileType.video && "mp4" || ".zip, .exe" }  
                               readOnly
                               placeholder='Select Audio/Photo/Document to Upload'
                        />
                    </div>
                    <FileList
                    files={addfiles}
                    shouldShowSpinner={null }
                    statusMessage={ null }
                    />
                     </div>
                    </div>
                </div>
            </div>

            <div className='card'>
                <div className='card-content'>
                    <div className='row'>
                    <div className="file-field input-field">
                    <div className="btn">
                        <span><i className="fas fa-file-alt" />&nbsp;Select File</span>
                        <input type="file"
                            
                        />
                    </div>
                    <div className="file-path-wrapper">
                        <input className="file-path validate"
                               type="text"
                               readOnly
                               
                               placeholder='Select 1 PDF file'
                        />
                    </div>
                    <p className='small-text'>Upload a File to automatically convert it to RichTextContent</p>
                </div>
                    <div>
                    <br></br>
                    <Editor
                    toolbarClassName="toolbarClassName"
                    wrapperClassName="wrapperClassName"
                    editorClassName="editorClassName"
                    />
                    </div>
                    </div>

                    <div className='row'>
                        
                    <button className='btn waves-effect waves-light right'
                                   
                    >
                    <i className="fas fa-plus-circle" />
                    &nbsp; Post Content
                    </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ManageClassContent);