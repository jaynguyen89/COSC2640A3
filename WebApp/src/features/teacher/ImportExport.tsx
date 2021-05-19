import React from 'react';
import {connect} from 'react-redux';
import _ from "lodash";
import {getSchedule, IImportExport, ISchedule} from "./redux/interfaces";
import {invokeUploadFileForImportRequest} from "../classroom/redux/actions";
import {invokeGetSchedulesProgressRequest} from "./redux/actions";
import * as classroomConstants from '../classroom/redux/constants';
import * as teacherConstants from './redux/constants';
import {defaultFileImport, IFileImport} from "../classroom/redux/interfaces";
import ImportData from "./components/ImportData";
import ScheduleList from "./components/ScheduleList";
import ExportData from "./components/ExportData";
import {checkSession, EMPTY_STATUS, IStatusMessage} from "../../providers/helpers";
import {clearAuthUser} from "../authentication/redux/actions";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    uploadFileImport: state.classroomStore.uploadFileImport,
    getSchedules: state.teacherStore.getSchedules
});

const mapActionsToProps = {
    invokeUploadFileForImportRequest,
    invokeGetSchedulesProgressRequest,
    clearAuthUser
};

const ImportExport = (props: IImportExport) => {
    const [selectedFile, setSelectedFile] = React.useState(defaultFileImport);
    const [fileFieldsEnabled, setFileFieldsEnabled] = React.useState(true);
    const [schedules, setSchedules] = React.useState(Array<ISchedule>());
    const [importMessage, setImportMessage] = React.useState(EMPTY_STATUS);
    const [scheduleMessage, setScheduleMessage] = React.useState(EMPTY_STATUS);

    React.useEffect(() => {
        props.invokeGetSchedulesProgressRequest(props.authUser);
    }, []);

    const handleFileSelect = (file: FileList | null, type: string) => {
        setSelectedFile({
            importType: type === 'classrooms' ? 0 : 1,
            fileForImport: (file && file[0]) || (null as unknown as File)
        } as IFileImport);
    }

    const attemptUploadingFileForImport = () => {
        if (selectedFile.fileForImport === null) {
            alert('You have selected no file to upload.');
            return;
        }

        setFileFieldsEnabled(false);
        props.invokeUploadFileForImportRequest(props.authUser, selectedFile);
    }

    React.useEffect(() => {
        if (props.uploadFileImport.action === classroomConstants.UPLOAD_JSON_CLASSROOMS_FILE_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setImportMessage, props.uploadFileImport.error?.message);

        if (props.uploadFileImport.action === classroomConstants.UPLOAD_JSON_CLASSROOMS_FILE_REQUEST_SUCCESS)
            if (props.uploadFileImport.payload === null)
                setImportMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.uploadFileImport.payload.result === 0)
                setImportMessage({ messages: props.uploadFileImport.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setImportMessage({ messages: [`The JSON file for importing ${ (selectedFile.importType === 0 && 'classrooms') || 'students' } has been uploaded.`], type: 'success' } as IStatusMessage);
                let newSchedule = getSchedule(props.uploadFileImport.payload.data);
                newSchedule.isForClassroom = selectedFile.importType === 0;

                let clone = _.cloneDeep(schedules);
                clone.push(newSchedule);
                setSchedules(clone);

                setSelectedFile(defaultFileImport);
                setImportMessage(EMPTY_STATUS);
            }

        setFileFieldsEnabled(true);
    }, [props.uploadFileImport]);

    React.useEffect(() => {
        if (props.getSchedules.action === teacherConstants.GET_IMPORT_SCHEDULES_PROGRESS_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setScheduleMessage, props.getSchedules.error?.message);

        if (props.getSchedules.action === teacherConstants.GET_IMPORT_SCHEDULES_PROGRESS_REQUEST_SUCCESS)
            if (props.getSchedules.payload === null)
                setScheduleMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.getSchedules.payload.result === 0)
                setScheduleMessage({ messages: props.getSchedules.payload.messages, type: 'error' } as IStatusMessage);
            else
                setSchedules(props.getSchedules.payload.data as Array<ISchedule>);
    }, [props.getSchedules]);

    return (
        <div className='row'>
            <div className='col s12'>
                <h5 className='title'>
                    <i className="fas fa-cloud-upload-alt" />
                    &nbsp;Import & Export Schedules
                </h5>
            </div>

            <div className='col s12'>
                <ImportData
                    selectedFile={ selectedFile }
                    handleSelection={ handleFileSelect }
                    attemptUploadingFileForImport={ attemptUploadingFileForImport }
                    enableButtons={ fileFieldsEnabled }
                    shouldShowSpinner={ props.uploadFileImport.action === classroomConstants.UPLOAD_JSON_CLASSROOMS_FILE_REQUEST_SENT }
                    statusMessage={ importMessage }
                    closeStatusMessage={ () => setImportMessage(EMPTY_STATUS) }
                />

                <ExportData />

                <ScheduleList
                    schedules={ schedules }
                    shouldShowSpinner={ props.getSchedules.action === teacherConstants.GET_IMPORT_SCHEDULES_PROGRESS_REQUEST_SENT }
                    statusMessage={ scheduleMessage }
                    closeStatusMessage={ () => setScheduleMessage(EMPTY_STATUS) }
                />
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ImportExport);
