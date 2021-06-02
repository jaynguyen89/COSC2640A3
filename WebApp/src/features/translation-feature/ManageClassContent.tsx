import React from 'react';
import {connect} from 'react-redux';
import _ from 'lodash';
import parse from 'html-react-parser';
import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import {defaultClassroom, IClassroomData} from "../classroom/redux/interfaces";
import ReactPlayer from 'react-player'
import {
    checkSession, createFileUrl,
    EMPTY_STATUS,
    EMPTY_STRING,
    FileTypes,
    IStatusMessage, modalOptions, removeGlobalMessage, setGlobalMessage, verifyFileTypeForSelectedFiles
} from "../../providers/helpers";
import ClassroomSummary from "./components/ClassroomSummary";
import {
    invokeAddFilesRequest,
    invokeAddRichContentRequest, invokeGetClassroomContentRequest,
    invokeImportRichContentRequest, invokeUpdateFilesRequest,
    invokeUpdateRichContentRequest
} from "./redux/actions";
import {
    defaultAddedFiles,
    defaultClassroomContent, defaultContent, defaultContentImport,
    defaultUpdatedFiles,
    emptyFile,
    IClassContent,
    IFile, IFileUpdating,
    IManageClassContent, IRichContent
} from "./redux/interfaces";
import * as contentConstants from './redux/constants';
import {clearAuthUser} from "../authentication/redux/actions";
import VideoSection from "./components/VideoSection";
import AttachmentSection from "./components/AttachmentSection";
import PhotoSection from "./components/PhotoSection";
import AudioSection from "./components/AudioSection";
import UpdateFilesModal from "./components/UpdateFilesModal";
import M from "materialize-css";
import $ from "jquery";
import AddFilesInput from "./components/AddFilesInput";
import ContentEditor from "./components/ContentEditor";
import ContextTranslation from "../translation/ContextTranslation";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    getContent: state.contentStore.getContent,
    addFiles: state.contentStore.addFiles,
    updateFiles: state.contentStore.updateFiles,
    addRichContent: state.contentStore.addRichContent,
    importRichContent: state.contentStore.importRichContent,
    updateRichContent: state.contentStore.updateRichContent,
});

const mapActionsToProps = {
    clearAuthUser,
    invokeGetClassroomContentRequest,
    invokeAddFilesRequest,
    invokeUpdateFilesRequest,
    invokeAddRichContentRequest,
    invokeImportRichContentRequest,
    invokeUpdateRichContentRequest,
};

const ManageClassContent = (props: IManageClassContent) => {
    const [classroom, setClassroom] = React.useState(defaultClassroom);
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [classroomContent, setClassroomContent] = React.useState(defaultClassroomContent);
    const [pageError, setPageError] = React.useState(EMPTY_STATUS);
    const [selectedVideo, setSelectedVideo] = React.useState(emptyFile);
    const [filesForUpdate, setFilesForUpdate] = React.useState(Array<IFile>());
    const [updatedFiles, setUpdatedFiles] = React.useState(defaultUpdatedFiles);
    const [addedFiles, setAddedFiles] = React.useState(defaultAddedFiles);
    const [richContent, setRichContent] = React.useState(defaultContent);
    const [isUpdatingRichContent, setIsUpdatingRichContent] = React.useState(false);
    const [filesForImport, setFilesForImport] = React.useState(defaultContentImport);
    const [isImportingRichContent, setIsImportingRichContent] = React.useState(false);

    React.useEffect(() => {
        const storedGlobalMessage = sessionStorage.getItem('globalMessage');
        if (storedGlobalMessage) {
            setStatusMessage(JSON.parse(storedGlobalMessage) as IStatusMessage);
            removeGlobalMessage();
        }

        M.Modal.init($('.modal'), {
            ...modalOptions,
            onCloseStart: () => setUpdatedFiles({
                classroomId: classroom.id,
                fileType: 0,
                uploadedFiles: null as unknown as FileList,
                removedFiles: Array<string>()
            } as IFileUpdating)
        });

        const classroomItem = localStorage.getItem('classroomDetailsItem');
        const storedClassroom = JSON.parse(classroomItem || EMPTY_STRING) as IClassroomData;
        setClassroom(storedClassroom || defaultClassroom);
        setRichContent({ ...richContent, classroomId: storedClassroom.id });

        props.invokeGetClassroomContentRequest(props.authUser, storedClassroom.id);
        setUpdatedFiles({
            classroomId: classroom.id,
            fileType: 0,
            uploadedFiles: null as unknown as FileList,
            removedFiles: Array<string>()
        } as IFileUpdating);
    }, []);

    React.useEffect(() => {
        if (props.getContent.action === contentConstants.GET_CLASSROOM_CONTENTS_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setPageError, props.getContent.error?.message);

        if (props.getContent.action === contentConstants.GET_CLASSROOM_CONTENTS_REQUEST_SUCCESS)
            if (props.getContent.payload === null)
                setPageError({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.getContent.payload.result === 0)
                setPageError({ messages: props.getContent.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setPageError(EMPTY_STATUS);
                setClassroomContent(props.getContent.payload.data as IClassContent);
                setSelectedVideo((props.getContent.payload.data as IClassContent).videos[0] || emptyFile);
            }
    }, [props.getContent]);

    const handleUpdateButtonsClicked = (fileType: string) => {
        if (fileType === 'rich-text') {
            setRichContent({ ...richContent, htmlContent: classroomContent.htmlContent } as IRichContent);
            setIsUpdatingRichContent(true);
            return;
        }

        if (fileType === FileTypes[0]) setFilesForUpdate(classroomContent.videos);
        if (fileType === FileTypes[1]) setFilesForUpdate(classroomContent.audios);
        if (fileType === FileTypes[2]) setFilesForUpdate(classroomContent.photos);
        if (fileType === FileTypes[3]) setFilesForUpdate(classroomContent.attachments);
        M.Modal.getInstance(document.querySelector('#updateFilesModal') as Element).open();
        setUpdatedFiles({ ...updatedFiles, fileType: FileTypes.indexOf(fileType) });
    }

    const handleImportContentBtnClicked = () => {
        setFilesForImport({ ...filesForImport, classroomId: classroom.id });
        setIsImportingRichContent(true);
    }

    const cancelUpdateMainContent = () => {
        setRichContent({ classroomId: classroom.id, htmlContent: EMPTY_STRING } as IRichContent);
        setIsUpdatingRichContent(false);
    }

    const handleModalRemoveBtnClicked = (fileId: string) => {
        let removedFiles = _.cloneDeep(updatedFiles.removedFiles);
        removedFiles.push(fileId);
        setUpdatedFiles({ ...updatedFiles, removedFiles: removedFiles });
    }

    const handleModalUnselectBtnClicked = (fileId: string) => {
        let removedFiles = _.cloneDeep(updatedFiles.removedFiles);
        _.remove(removedFiles, removedFileId => removedFileId === fileId);
        setUpdatedFiles({ ...updatedFiles, removedFiles: removedFiles });
    }

    const handleModalFilesInput = (files: FileList) => {
        if (files.length === 0) {
            setUpdatedFiles({ ...updatedFiles, uploadedFiles: null as unknown as FileList });
            return;
        }

        const filesVerification: string | null = verifyFileTypeForSelectedFiles(files, updatedFiles.fileType);
        if (filesVerification) {
            alert(filesVerification);
            return;
        }

        setUpdatedFiles({ ...updatedFiles, uploadedFiles: files });
    }

    const handleAddingFiles = (files: FileList, fileType: number) => {
        if (files.length === 0) {
            setAddedFiles(defaultAddedFiles);
            return;
        }

        const filesVerification: string | null = verifyFileTypeForSelectedFiles(files, fileType);
        if (filesVerification) {
            alert(filesVerification);
            return;
        }

        setAddedFiles({ ...addedFiles, fileType: fileType, classroomId: classroom.id });
    }

    const getNewRichTextContent = (newContent: any) => setRichContent({ ...richContent, htmlContent: newContent });

    const handleImportingFiles = (files: FileList) => {
        if (files.length === 0) {
            setFilesForImport(defaultContentImport);
            return;
        }

        const filesVerification: string | null = verifyFileTypeForSelectedFiles(files, 2);
        if (filesVerification) {
            alert(filesVerification);
            return;
        }

        setFilesForImport({ ...filesForImport, filesForImport: files });
    }
    
    const handleVideoThumbnailSelected = (videoId: string) => {
        const videoIndex = _.findIndex(classroomContent.videos, video => video.id === videoId);
        if (videoIndex === 0 && selectedVideo.id === videoId) return;

        setSelectedVideo(classroomContent.videos[videoIndex]);
    }

    const handleModalUpdateBtnClicked = () => {
        if ((updatedFiles.uploadedFiles === null || updatedFiles.uploadedFiles.length === 0) && updatedFiles.removedFiles.length === 0) {
            alert('You have made no changes or your changes was not applied due to errors in file selection. Request won\'t be sent.');
            return;
        }

        const clone = _.cloneDeep(updatedFiles);
        clone.classroomId = classroom.id;

        props.invokeUpdateFilesRequest(props.authUser, clone);
        setFilesForUpdate(Array<IFile>());
        setUpdatedFiles(defaultUpdatedFiles);
        M.Modal.getInstance(document.querySelector('#updateFilesModal') as Element).close();
    }

    const handleUploadBtnClicked = () => {
        if (addedFiles.uploadedFiles === null || addedFiles.uploadedFiles.length === 0) {
            alert('You have made no changes or your changes was not applied due to errors in file selection. Request won\'t be sent.');
            return;
        }

        const clone = _.cloneDeep(addedFiles);
        clone.classroomId = classroom.id;

        props.invokeAddFilesRequest(props.authUser, clone);
        setAddedFiles(defaultAddedFiles);
    }

    const attemptImportingMainContent = () => {
        if (filesForImport.filesForImport === null || filesForImport.filesForImport.length === 0) {
            alert('You have made no changes or your changes was not applied due to errors in file selection. Request won\'t be sent.');
            return;
        }

        const clone = _.cloneDeep(filesForImport);
        clone.classroomId = classroom.id;

        props.invokeImportRichContentRequest(props.authUser, clone);
        setFilesForImport(defaultContentImport);
        setIsImportingRichContent(false);
    }

    const attemptUpdateRichContent = () => {
        if (richContent.htmlContent === classroomContent.htmlContent) {
            alert('You have made no changes to the main content. Request won\'t be sent.');
            return;
        }

        const clone = _.cloneDeep(richContent);
        clone.classroomId = classroom.id;

        props.invokeUpdateRichContentRequest(props.authUser, clone);
        setRichContent(defaultContent);
        setIsUpdatingRichContent(false);
    }

    React.useEffect(() => {
        if (props.addFiles.action === contentConstants.ADD_FILES_TO_CLASSROOM_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.addFiles.error?.message);

        if (props.addFiles.action === contentConstants.ADD_FILES_TO_CLASSROOM_REQUEST_SUCCESS)
            if (props.addFiles.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.addFiles.payload.result === 0)
                setStatusMessage({ messages: props.addFiles.payload.messages, type: 'error' } as IStatusMessage);
            else {
                props.invokeGetClassroomContentRequest(props.authUser, classroom.id);
                setGlobalMessage({ messages: ['Files have been added to classroom successfully.'], type: 'success', closeAlert: () => setStatusMessage(EMPTY_STATUS) } as IStatusMessage);
            }
    }, [props.addFiles]);

    React.useEffect(() => {
        if (props.updateFiles.action === contentConstants.UPDATE_OR_REMOVE_FILES_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.updateFiles.error?.message);

        if (props.updateFiles.action === contentConstants.UPDATE_OR_REMOVE_FILES_REQUEST_SUCCESS)
            if (props.updateFiles.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.updateFiles.payload.result === 0)
                setStatusMessage({ messages: props.updateFiles.payload.messages, type: 'error' } as IStatusMessage);
            else {
                props.invokeGetClassroomContentRequest(props.authUser, classroom.id);
                setGlobalMessage({ messages: ['Files have been added and/or removed successfully.'], type: 'success', closeAlert: () => setStatusMessage(EMPTY_STATUS) } as IStatusMessage);
            }
    }, [props.updateFiles]);

    React.useEffect(() => {
        if (props.addRichContent.action === contentConstants.ADD_RICH_CONTENT_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.addRichContent.error?.message);

        if (props.addRichContent.action === contentConstants.ADD_RICH_CONTENT_REQUEST_SUCCESS)
            if (props.addRichContent.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.addRichContent.payload.result === 0)
                setStatusMessage({ messages: props.addRichContent.payload.messages, type: 'error' } as IStatusMessage);
            else {
                props.invokeGetClassroomContentRequest(props.authUser, classroom.id);
                setGlobalMessage({ messages: ['The main content has been added successfully.'], type: 'success', closeAlert: () => setStatusMessage(EMPTY_STATUS) } as IStatusMessage);
            }
    }, [props.addRichContent]);

    React.useEffect(() => {
        if (props.importRichContent.action === contentConstants.IMPORT_RICH_CONTENT_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.importRichContent.error?.message);

        if (props.importRichContent.action === contentConstants.IMPORT_RICH_CONTENT_REQUEST_SUCCESS)
            if (props.importRichContent.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.importRichContent.payload.result === 0)
                setStatusMessage({ messages: props.importRichContent.payload.messages, type: 'error' } as IStatusMessage);
            else {
                props.invokeGetClassroomContentRequest(props.authUser, classroom.id);
                setGlobalMessage({ messages: ['The main content has been imported successfully.'], type: 'success', closeAlert: () => setStatusMessage(EMPTY_STATUS) } as IStatusMessage);
            }
    }, [props.importRichContent]);

    React.useEffect(() => {
        if (props.updateRichContent.action === contentConstants.UPDATE_RICH_CONTENT_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.updateRichContent.error?.message);

        if (props.updateRichContent.action === contentConstants.UPDATE_RICH_CONTENT_REQUEST_SUCCESS)
            if (props.updateRichContent.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.updateRichContent.payload.result === 0)
                setStatusMessage({ messages: props.updateRichContent.payload.messages, type: 'error' } as IStatusMessage);
            else {
                props.invokeGetClassroomContentRequest(props.authUser, classroom.id);
                setGlobalMessage({ messages: ['The main content has been updated successfully.'], type: 'success', closeAlert: () => setStatusMessage(EMPTY_STATUS) } as IStatusMessage);
            }
    }, [props.updateRichContent]);

    return (
        <div className='container' style={{ marginTop: '4em' }}>
            {
                (
                    props.getContent.action === contentConstants.GET_CLASSROOM_CONTENTS_REQUEST_SENT ||
                    props.addFiles.action === contentConstants.ADD_FILES_TO_CLASSROOM_REQUEST_SENT ||
                    props.updateFiles.action === contentConstants.UPDATE_OR_REMOVE_FILES_REQUEST_SENT ||
                    props.addRichContent.action === contentConstants.ADD_RICH_CONTENT_REQUEST_SENT ||
                    props.importRichContent.action === contentConstants.IMPORT_RICH_CONTENT_REQUEST_SENT ||
                    props.updateRichContent.action === contentConstants.UPDATE_RICH_CONTENT_REQUEST_SENT
                ) && <Spinner />
            }

            { pageError.messages.length !== 0 && <Alert { ...pageError } closeAlert={ () => setPageError(EMPTY_STATUS) } /> }
            <Alert { ...statusMessage } closeAlert={ () => setStatusMessage(EMPTY_STATUS) } />

            {
                pageError.messages.length === 0 &&
                <>
                    <div className='row' style={{ marginBottom: 0 }}>
                        <div className='col s12 center-align'>
                            <h4 style={{ marginTop: 0 }}>{ classroom.className }</h4>
                            <hr />
                        </div>
                    </div>

                    <div className='row'>
                        <div className='col m9 s12' style={{ padding: 0 }}>
                            <ReactPlayer
                                url={ classroomContent.videos.length === 0 ? 'any.mp4' : createFileUrl(selectedVideo, classroom.id) }
                                controls={ true }
                                width='100%'
                                height='100%'
                                stopOnUnmount={ true }
                            />

                            <div className='card horizontal'>
                                <div className='horizontal-scroller'>
                                    <VideoSection
                                        videos={ classroomContent.videos }
                                        selectedVideo={ selectedVideo }
                                        classroomId={ classroom.id }
                                        onSelectVideo={ handleVideoThumbnailSelected }
                                    />
                                </div>
                            </div>

                            {
                                props.authUser.role === 1 &&
                                <div className='col s12'>
                                    {
                                        (
                                            classroomContent.videos &&
                                            <button className='btn waves-effect waves-light' style={{ marginLeft: '1em' }}
                                                    onClick={ () => handleUpdateButtonsClicked(FileTypes[0]) }
                                            >
                                                <i className="fas fa-pen" />&nbsp;
                                                Update Videos
                                            </button>
                                        ) ||
                                        <AddFilesInput
                                            files={ addedFiles.uploadedFiles }
                                            fileType={ 0 }
                                            handleFilesInput={ handleAddingFiles }
                                            confirmAdding={ handleUploadBtnClicked }
                                        />
                                    }
                                </div>
                            }
                        </div>
                        <div className='col m3 s12' style={{ paddingLeft: '1em' }}>
                            <ClassroomSummary classroom={ classroom } />
                        </div>
                    </div>

                    <div className='row'>
                        <div className='col m9 s12'>
                            <h5 className='title'>
                                <i className="fas fa-compress-arrows-alt" />
                                &nbsp;Main contents
                            </h5>

                            <div className='row'>
                                <ContextTranslation>
                                    <div className='col s12' style={{ textAlign: 'justify' }}>
                                        {
                                            (
                                                classroomContent.htmlContent && (
                                                    (
                                                        !isUpdatingRichContent && parse(classroomContent.htmlContent)
                                                    ) ||
                                                    <ContentEditor content={ richContent.htmlContent } informChanges={ getNewRichTextContent } />
                                                )
                                            ) ||
                                            <ContentEditor content={ richContent.htmlContent } informChanges={ getNewRichTextContent } />
                                        }
                                    </div>
                                </ContextTranslation>

                                {
                                    props.authUser.role === 1 &&
                                    <div className='col s12'>
                                        {
                                            (
                                                classroomContent.htmlContent && (
                                                    (
                                                        !isUpdatingRichContent &&
                                                        <button className='btn waves-effect waves-light'
                                                                onClick={ () => handleUpdateButtonsClicked('rich-text') }
                                                        >
                                                            <i className="fas fa-pen" />&nbsp;
                                                            Update Main Content
                                                        </button>
                                                    ) ||
                                                    <>
                                                        <button className='btn waves-effect waves-light'
                                                                onClick={ () => attemptUpdateRichContent() }
                                                        >
                                                            <i className="fas fa-pen" />&nbsp;
                                                            Update
                                                        </button>

                                                        <button className='btn waves-effect waves-light amber' style={{ marginLeft: '1em' }}
                                                                onClick={ () => cancelUpdateMainContent() }
                                                        >
                                                            <i className="fas fa-undo" />&nbsp;
                                                            Cancel
                                                        </button>
                                                    </>
                                                )
                                            ) ||
                                            <button className='btn waves-effect waves-light'
                                                    onClick={ () => attemptUpdateRichContent() }
                                            >
                                                <i className="fas fa-quote-left" />&nbsp;
                                                Add Main Content
                                            </button>
                                        }

                                        {
                                            (
                                                !isImportingRichContent &&
                                                <button className='btn waves-effect waves-light right' style={{ marginLeft: '1em' }}
                                                        onClick={ () => handleImportContentBtnClicked() }
                                                >
                                                    <i className="fas fa-file-import" />&nbsp;
                                                    Import Main Content
                                                </button>
                                            ) ||
                                            <div className='row'>
                                                <div className='col s12'>
                                                    <p className='title'>Select photos to upload and import main contents automatically:</p>
                                                    <button className='btn btn-small waves-effect waves-light right amber'
                                                            onClick={ () => {
                                                                setFilesForImport(defaultContentImport);
                                                                setIsImportingRichContent(false);
                                                            }}
                                                    >
                                                        Cancel
                                                    </button>
                                                    <AddFilesInput
                                                        files={ filesForImport.filesForImport }
                                                        fileType={ 2 }
                                                        handleFilesInput={ handleImportingFiles }
                                                        confirmAdding={ attemptImportingMainContent }
                                                    />
                                                </div>
                                            </div>
                                        }
                                    </div>
                                }
                            </div>
                        </div>

                        <div className='col m3 s12'>
                            <h5 className='title'>
                                <i className="fas fa-paperclip" />
                                &nbsp;Attachments
                            </h5>

                            <div className='row'>
                                <AttachmentSection attachments={ classroomContent.attachments } classroomId={ classroom.id } />

                                {
                                    props.authUser.role === 1 &&
                                    <div className='col s12'>
                                        {
                                            (
                                                classroomContent.attachments &&
                                                <button className='btn waves-effect waves-light' style={{ marginLeft: '1em' }}
                                                        onClick={ () => handleUpdateButtonsClicked(FileTypes[3]) }
                                                >
                                                    <i className="fas fa-pen" />&nbsp;
                                                    Update Files
                                                </button>
                                            ) ||
                                            <AddFilesInput
                                                files={ addedFiles.uploadedFiles }
                                                fileType={ 2 }
                                                handleFilesInput={ handleAddingFiles }
                                                confirmAdding={ handleUploadBtnClicked }
                                            />
                                        }
                                    </div>
                                }
                            </div>
                        </div>
                    </div>

                    <div className='row'>
                        <div className='col m8 s12'>
                            <h5 className='title'>
                                <i className="fas fa-image" />
                                &nbsp;Attached images
                            </h5>

                            <div className='row'>
                                <PhotoSection photos={ classroomContent.photos } classroomId={ classroom.id } />

                                {
                                    props.authUser.role === 1 &&
                                    <div className='col s12'>
                                        {
                                            (
                                                classroomContent.photos &&
                                                <button className='btn waves-effect waves-light' style={{ marginLeft: '1em' }}
                                                        onClick={ () => handleUpdateButtonsClicked(FileTypes[2]) }
                                                >
                                                    <i className="fas fa-pen" />&nbsp;
                                                    Update Photos
                                                </button>
                                            ) ||
                                            <AddFilesInput
                                                files={ addedFiles.uploadedFiles }
                                                fileType={ 2 }
                                                handleFilesInput={ handleAddingFiles }
                                                confirmAdding={ handleUploadBtnClicked }
                                            />
                                        }
                                    </div>
                                }
                            </div>
                        </div>

                        <div className='col m4 s12'>
                            <h5 className='title'>
                                <i className="fas fa-headset" />
                                &nbsp;Audio recordings
                            </h5>

                            <div className='row'>
                                <div className='col s12'>
                                    <AudioSection audios={ classroomContent.audios } classroomId={ classroom.id } />

                                    {
                                        props.authUser.role === 1 &&
                                        <div className='col s12'>
                                            {
                                                (
                                                    classroomContent.audios &&
                                                    <button className='btn waves-effect waves-light' style={{ marginLeft: '1em' }}
                                                            onClick={ () => handleUpdateButtonsClicked(FileTypes[1]) }
                                                    >
                                                        <i className="fas fa-pen" />&nbsp;
                                                        Update Audios
                                                    </button>
                                                ) ||
                                                <AddFilesInput
                                                    files={ addedFiles.uploadedFiles }
                                                    fileType={ 1 }
                                                    handleFilesInput={ handleAddingFiles }
                                                    confirmAdding={ handleUploadBtnClicked }
                                                />
                                            }
                                        </div>
                                    }
                                </div>
                            </div>
                        </div>

                        <UpdateFilesModal
                            classroomId={ classroom.id }
                            files={ filesForUpdate }
                            fileType={ updatedFiles.fileType }
                            selectedFiles={ updatedFiles.removedFiles }
                            handleFilesInput={ handleModalFilesInput }
                            removeBtnClick={ handleModalRemoveBtnClicked }
                            unselectBtnClick={ handleModalUnselectBtnClicked }
                            updateBtnClick={ handleModalUpdateBtnClicked }
                        />
                    </div>
                </>
            }

            <br />
            <br />
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ManageClassContent);