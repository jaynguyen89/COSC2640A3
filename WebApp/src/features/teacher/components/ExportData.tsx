import React from 'react';
import {connect} from 'react-redux';
import _ from 'lodash';
import $ from 'jquery';
import M from 'materialize-css';
import Spinner from "../../../shared/Spinner";
import Alert from "../../../shared/Alert";
import {
    checkSession,
    EMPTY_STATUS, EMPTY_STRING, IStatusMessage,
    modalOptions
} from "../../../providers/helpers";
import {invokeGetAllTeacherClassroomsRequest} from "../../classroom/redux/actions";
import {IClassroomData} from "../../classroom/redux/interfaces";
import * as classroomConstants from '../../classroom/redux/constants';
import * as teacherConstants from '../redux/constants';
import {clearAuthUser} from "../../authentication/redux/actions";
import {invokeExportClassroomsRequest, invokeExportStudentsRequest} from "../redux/actions";
import {IDataExport, IExportData} from "../redux/interfaces";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    getTeacherClassrooms: state.classroomStore.getTeacherClassrooms,
    exportClassrooms: state.teacherStore.exportClassrooms,
    exportStudents: state.teacherStore.exportStudents,
});

const mapActionsToProps = {
    invokeGetAllTeacherClassroomsRequest,
    invokeExportClassroomsRequest,
    invokeExportStudentsRequest,
    clearAuthUser
}

const ExportData = (props: IExportData) => {
    const [classrooms, setClassrooms] = React.useState(Array<IClassroomData>());
    const [completedClassrooms, setCompletedClassrooms] = React.useState(Array<IClassroomData>());
    const [buttonsEnabled, setButtonsEnabled] = React.useState(false);
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [selectedIds, setSelectedIds] = React.useState(Array<string>());
    const [exportType, setExportType] = React.useState(EMPTY_STRING);

    React.useEffect(() => {
        M.Modal.init($('.modal'), modalOptions);
        props.invokeGetAllTeacherClassroomsRequest(props.authUser, null);
    }, []);

    React.useEffect(() => {
        if (props.getTeacherClassrooms.action === classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.getTeacherClassrooms.error?.message);

        if (props.getTeacherClassrooms.action === classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_SUCCESS)
            if (props.getTeacherClassrooms.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.getTeacherClassrooms.payload.result === 0)
                setStatusMessage({ messages: props.getTeacherClassrooms.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setButtonsEnabled(true);
                setClassrooms((props.getTeacherClassrooms.payload.data as any).classrooms as Array<IClassroomData>);
                if ((props.getTeacherClassrooms.payload.data as any).completedClassrooms)
                    setCompletedClassrooms((props.getTeacherClassrooms.payload.data as any).completedClassrooms);
            }
    }, [props.getTeacherClassrooms]);

    const handleCheckboxClicked = (classroomId: string, isChecked: boolean) => {
        let clone = _.cloneDeep(selectedIds);

        if (isChecked) clone.push(classroomId);
        else _.remove(clone, selectedId => selectedId === classroomId);

        setSelectedIds(clone);
    }

    const attemptExportingData = () => {
        if (selectedIds.length === 0) {
            alert('No classroom has been selected to export data from.');
            return;
        }

        M.Modal.getInstance(document.querySelector('#classroomListModal') as Element).close();
        setButtonsEnabled(false);
        if (exportType === 'classrooms') props.invokeExportClassroomsRequest(props.authUser, { classroomIds: selectedIds } as IDataExport);
        else props.invokeExportStudentsRequest(props.authUser, { classroomIds: selectedIds } as IDataExport);
    }

    React.useEffect(() => {
        setButtonsEnabled(true);

        if (props.exportClassrooms.action === teacherConstants.EXPORT_CLASSROOMS_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.exportClassrooms.error?.message);

        if (props.exportClassrooms.action === teacherConstants.EXPORT_CLASSROOMS_REQUEST_SUCCESS)
            if (props.exportClassrooms.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.exportClassrooms.payload.result === 0)
                setStatusMessage({ messages: props.exportClassrooms.payload.messages, type: 'error' } as IStatusMessage);
            else
                saveFile(JSON.stringify(props.exportClassrooms.payload));
    }, [props.exportClassrooms]);

    React.useEffect(() => {
        setButtonsEnabled(true);

        if (props.exportStudents.action === teacherConstants.EXPORT_STUDENTS_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.exportStudents.error?.message);

        if (props.exportStudents.action === teacherConstants.EXPORT_STUDENTS_REQUEST_SUCCESS)
            if (props.exportStudents.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.exportStudents.payload.result === 0)
                setStatusMessage({ messages: props.exportStudents.payload.messages, type: 'error' } as IStatusMessage);
            else
                saveFile(JSON.stringify(props.exportStudents.payload));
    }, [props.exportStudents]);

    const saveFile = (content: string) => {
        const downloadLink = document.createElement('a');
        downloadLink.href = window.URL.createObjectURL(new Blob([content], { type: 'text/plain;charset=utf-8' }));
        downloadLink.setAttribute(
            'download',
            (exportType === 'classrooms' && `${ props.authUser.accountId }_classrooms_exports.json`) || `${ props.authUser.accountId }_students_exports.json`
        );

        document.body.appendChild(downloadLink);
        downloadLink.click();
        document.body.removeChild(downloadLink);

        setExportType(EMPTY_STRING);
        setSelectedIds(Array<string>());
    }

    return (
        <div className='row'>
            <div className='col s12'>
                <p className='title section-header'>
                    <i className="fas fa-angle-right" />
                    &nbsp;Export your classrooms & students
                </p>
                {
                    (
                        props.getTeacherClassrooms.action === classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_SENT ||
                        props.exportClassrooms.action === teacherConstants.EXPORT_CLASSROOMS_REQUEST_SENT ||
                        props.exportStudents.action === teacherConstants.EXPORT_STUDENTS_REQUEST_SENT
                    ) && <Spinner />
                }
                <Alert { ...statusMessage } closeAlert={ () => setStatusMessage(EMPTY_STATUS) } />

                <div className="row center-align" style={{ marginTop: '1em' }}>
                    <div className='col s6'>
                        <button className={ (buttonsEnabled && 'btn waves-effect waves-light') || 'btn disabled' }
                                onClick={ () => {
                                    setExportType('classrooms');
                                    M.Modal.getInstance(document.querySelector('#classroomListModal') as Element).open()
                                }}
                        >
                            <i className="fab fa-buromobelexperte" />
                            &nbsp;Export classrooms
                        </button>
                    </div>

                    <div className='col s6'>
                        <button className={ (buttonsEnabled && 'btn waves-effect waves-light') || 'btn disabled' }
                                onClick={ () => {
                                    setExportType('students');
                                    M.Modal.getInstance(document.querySelector('#classroomListModal') as Element).open()
                                }}
                        >
                            <i className="fas fa-user-graduate" />
                            &nbsp;Export students
                        </button>
                    </div>
                </div>
            </div>

            <div className='modal' id='classroomListModal'>
                <div className='modal-content'>
                    <p className='section-header'>
                        <i className="fas fa-hand-pointer" />&nbsp;&nbsp;
                        Select classrooms to export
                    </p>

                    <table>
                        <thead>
                            <tr>
                                <td>Classroom Name</td>
                                <td>Price</td>
                                <td>Enrolments</td>
                                <td>Status</td>
                                <td>Select</td>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                classrooms.map(classroom =>
                                    <tr key={ classroom.id }>
                                        <td>{ classroom.className }</td>
                                        <td>{ classroom.price }</td>
                                        <td>{ classroom.enrolmentsCount }</td>
                                        <td style={{ color: '#4caf50' }}>Active</td>
                                        <td>
                                            <label>
                                                <input type="checkbox"
                                                       checked={ selectedIds.indexOf(classroom.id) !== -1 }
                                                       onChange={ e => handleCheckboxClicked(classroom.id, e.target.checked) }
                                                />
                                                <span />
                                            </label>
                                        </td>
                                    </tr>
                                )
                            }

                            {
                                completedClassrooms.map(classroom =>
                                    <tr key={ classroom.id }>
                                        <td>{ classroom.className }</td>
                                        <td>{ classroom.price }</td>
                                        <td>{ classroom.enrolmentsCount }</td>
                                        <td style={{ color: '#ff9800' }}>Completed</td>
                                        <td>
                                            <label>
                                                <input type="checkbox"
                                                       checked={ selectedIds.indexOf(classroom.id) !== -1 }
                                                       onChange={ e => handleCheckboxClicked(classroom.id, e.target.checked) }
                                                />
                                                <span />
                                            </label>
                                        </td>
                                    </tr>
                                )
                            }
                        </tbody>
                    </table>

                    <div className='col s12 center-align' style={{ marginTop: '1em', marginBottom: '1em' }}>
                        <button className='btn waves-effect waves-light'
                                onClick={ attemptExportingData }
                        >
                            <i className="fas fa-file-export" />
                            &nbsp;Export from { (selectedIds && selectedIds.length) || 0 } selected classrooms
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
)(ExportData);