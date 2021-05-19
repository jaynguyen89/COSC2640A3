import React from 'react';
import {connect} from 'react-redux';
import _ from 'lodash';
import $ from 'jquery';
import M from 'materialize-css';
import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import {defaultMarks, IManageEnrolments, IUpdateMarks} from "./redux/interfaces";
import {invokeGetEnrolmentsByClassroomRequest} from "../classroom/redux/actions";
import {defaultMarkBreakdown, IEnrolment, IMarkBreakdown} from "../student/redux/interfaces";
import {
    calculateOverallMarks,
    checkSession,
    checkUnenrolmentResult,
    EMPTY_STATUS,
    IStatusMessage,
    modalOptions
} from "../../providers/helpers";
import * as classroomConstants from '../classroom/redux/constants';
import * as teacherConstants from './redux/constants';
import * as studentConstants from "../student/redux/constants";
import {defaultClassroom, IClassroomData} from "../classroom/redux/interfaces";
import ClassroomInfo from "../classroom/components/ClassroomInfo";
import EnrolmentInfo from "../student/EnrolmentInfo";
import {invokeUnenrolFromClassroomRequest} from "../student/redux/actions";
import {clearAuthUser} from "../authentication/redux/actions";
import {invokeAddMarksToEnrolmentRequest} from "./redux/actions";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    getClassroomEnrolments: state.classroomStore.getClassroomEnrolments,
    unenrolClassroom: state.studentStore.unenrolClassroom,
    updateMarks: state.teacherStore.updateMarks
});

const mapActionsToProps = {
    invokeGetEnrolmentsByClassroomRequest,
    invokeUnenrolFromClassroomRequest,
    clearAuthUser,
    invokeAddMarksToEnrolmentRequest
};

const ManageEnrolments = (props: IManageEnrolments) => {
    const [classroom, setClassroom] = React.useState(defaultClassroom);
    const [enrolments, setEnrolments] = React.useState(Array<IEnrolment>());
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [marksToUpdate, setMarksToUpdate] = React.useState(defaultMarks);

    React.useEffect(() => {
        const storedClassroomItem = localStorage.getItem('classroom_ManageEnrolments');
        localStorage.removeItem('classroomId_ManageEnrolments');

        if (storedClassroomItem) {
            const storedClassroom = JSON.parse(storedClassroomItem) as IClassroomData;
            setClassroom(storedClassroom);
            props.invokeGetEnrolmentsByClassroomRequest(props.authUser, storedClassroom.id as string);
        }
    }, []);

    React.useEffect(() => {
        M.Modal.init($('.modal'), {
            ...modalOptions,
            onCloseStart: () => setMarksToUpdate(defaultMarks)
        });
        M.Tabs.init($('.tabs'), { duration: 250, swipeable: false });
        M.Collapsible.init($('.collapsible'), { accordion: true });
    }, [enrolments]);

    React.useEffect(() => {
        if (props.getClassroomEnrolments.action === classroomConstants.GET_ENROLMENTS_BY_CLASSROOM_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.getClassroomEnrolments.error?.message);

        if (props.getClassroomEnrolments.action === classroomConstants.GET_ENROLMENTS_BY_CLASSROOM_REQUEST_SUCCESS)
            if (props.getClassroomEnrolments.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.getClassroomEnrolments.payload.result === 0)
                setStatusMessage({ messages: props.getClassroomEnrolments.payload.messages, type: 'error' } as IStatusMessage);
            else
                setEnrolments(props.getClassroomEnrolments.payload.data as Array<IEnrolment>);
    }, [props.getClassroomEnrolments]);

    React.useEffect(() => {
        const result = checkUnenrolmentResult(props.clearAuthUser, setStatusMessage, props.unenrolClassroom, _.cloneDeep(enrolments));
        if (result) setEnrolments(result);
    }, [props.unenrolClassroom]);

    const handleUpdateMarksBtnClicked = (enrolmentId: string, markBreakdowns: Array<IMarkBreakdown>) => {
        setMarksToUpdate({
            enrolmentId: enrolmentId,
            markBreakdowns: markBreakdowns || defaultMarks
        } as IUpdateMarks);

        M.Modal.getInstance(document.querySelector('.modal') as Element).open();
    }

    const handleResetBtnClicked = (entryIndex: number) => {
        let clone = _.cloneDeep(marksToUpdate.markBreakdowns);
        clone[entryIndex] = defaultMarkBreakdown;
        setMarksToUpdate({ ...marksToUpdate, markBreakdowns: clone });
    }

    const handleRemoveEntryBtnClicked = (entryIndex: number) => {
        let clone = _.cloneDeep(marksToUpdate.markBreakdowns);
        clone.splice(entryIndex, 1);
        setMarksToUpdate({ ...marksToUpdate, markBreakdowns: clone });
    }

    const handleAddEntryBtnClicked = () => {
        let clone = _.cloneDeep(marksToUpdate.markBreakdowns);
        clone.push(defaultMarkBreakdown);
        setMarksToUpdate({ ...marksToUpdate, markBreakdowns: clone });
    }

    const updateMarkEntry = (entryIndex: number, field: string, value: string) => {
        let clone = _.cloneDeep(marksToUpdate.markBreakdowns);

        let marks = clone[entryIndex];
        if (field === 'taskName') marks.taskName = value;
        if (field === 'totalMarks') marks.totalMarks = Number(value);
        if (field === 'rewardedMarks') marks.rewardedMarks = Number(value);
        if (field === 'comment') marks.comment = value;

        clone[entryIndex] = marks;
        setMarksToUpdate({ ...marksToUpdate, markBreakdowns: clone });
    }

    const attemptUpdatingMarks = () => {
        if (marksToUpdate.markBreakdowns.length === 0) {
            alert('No marks have been added. Please add at least 1 marking entry.');
            return;
        }

        props.invokeAddMarksToEnrolmentRequest(props.authUser, marksToUpdate);
    }

    React.useEffect(() => {
        if (props.updateMarks.action === teacherConstants.ADD_MARKS_TO_ENROLMENT_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.updateMarks.error?.message);

        if (props.updateMarks.action === teacherConstants.ADD_MARKS_TO_ENROLMENT_REQUEST_SUCCESS)
            if (props.updateMarks.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.updateMarks.payload.result === 0)
                setStatusMessage({ messages: props.updateMarks.payload.messages, type: 'error' } as IStatusMessage);
            else {
                let clone = _.cloneDeep(enrolments);
                const updatedEnrolmentIndex = _.findIndex(clone, enrolment => enrolment.id === marksToUpdate.enrolmentId);
                let enrolmentToUpdate = _.remove(clone, enrolment => enrolment.id === marksToUpdate.enrolmentId)[0];

                if (enrolmentToUpdate) {
                    enrolmentToUpdate.marksDetail.markBreakdowns = marksToUpdate.markBreakdowns;
                    enrolmentToUpdate.marksDetail.overallMarks = calculateOverallMarks(marksToUpdate.markBreakdowns);
                }

                clone.splice(updatedEnrolmentIndex, 0, enrolmentToUpdate);
                setEnrolments(clone);

                setStatusMessage({ messages: [`The marks have been updated for enrolment by ${ enrolmentToUpdate.student.preferredName }.`], type: 'success' } as IStatusMessage);
                M.Modal.getInstance(document.querySelector('.modal') as Element).close();
            }
    }, [props.updateMarks]);

    return (
        <div className='container' style={{ marginTop: '4em' }}>
            <div className='row' style={{ marginBottom: 0 }}>
                <div className='col s12 center-align'>
                    <h4 style={{ marginTop: 0 }}>{ classroom.className }</h4>
                    <hr />
                </div>

                {
                    (
                        props.getClassroomEnrolments.action === classroomConstants.GET_ENROLMENTS_BY_CLASSROOM_REQUEST_SENT ||
                        props.updateMarks.action === teacherConstants.ADD_MARKS_TO_ENROLMENT_REQUEST_SENT
                    ) && <Spinner />
                }
                <Alert { ...statusMessage } closeAlert={ () => setStatusMessage(EMPTY_STATUS) } />
            </div>

            <div className='row'>
                <div className='col s12'>
                    <ClassroomInfo classroom={ classroom } />
                </div>

                {
                    (
                        enrolments.length === 0 &&
                        <div className='section-header center-align'>This classroom hasn't had any enrolment. Please select another classroom.</div>
                    ) ||
                    enrolments.map(enrolment =>
                        <div className='col l4 m6 s12' key={ enrolment.id }>
                            <EnrolmentInfo
                                enrolment={ enrolment }
                                handleUnenrolBtn={ () => props.invokeUnenrolFromClassroomRequest(props.authUser, enrolment.id) }
                                handleUpdateMarksBtn={ () => handleUpdateMarksBtnClicked(enrolment.id, enrolment.marksDetail.markBreakdowns) }
                            />
                        </div>
                    )
                }
            </div>

            <div className='modal' id='updateMarksModal'>
                <div className='row'>
                    <p className='col s12 section-header'>
                        <i className="fas fa-pen-nib" />&nbsp;
                        Update Marks
                    </p>

                    <div className='col s12'>
                        {
                            marksToUpdate.markBreakdowns.map((marks, i) =>
                                <div className='card' key={ i }>
                                    <div className='card-content'>
                                        <div className='row' style={{ margin: 0 }}>
                                            <div className='input-field col s12'>
                                                <i className='material-icons prefix'>task</i>
                                                <input id={ `taskName${ i }` }
                                                       type='text'
                                                       className='validate'
                                                       value={ marks.taskName }
                                                       onChange={ e => updateMarkEntry(i, 'taskName', e.target.value) }
                                                />
                                                <label htmlFor={ `taskName${ i }` }>Task Name</label>
                                            </div>

                                            <div className='input-field col s6'>
                                                <i className='material-icons prefix'>dialpad</i>
                                                <input id={ `totalMarks${ i }` } type='number' step='1'
                                                       className='validate'
                                                       value={ marks.totalMarks }
                                                       onChange={ e => updateMarkEntry(i, 'totalMarks', e.target.value) }
                                                />
                                                <label htmlFor={ `totalMarks${ i }` }>Total Marks</label>
                                            </div>

                                            <div className='input-field col s6'>
                                                <i className='material-icons prefix'>dialpad</i>
                                                <input id={ `rewardedMarks${ i }` } type='number' step='1'
                                                       className='validate'
                                                       value={ marks.rewardedMarks }
                                                       onChange={ e => updateMarkEntry(i, 'rewardedMarks', e.target.value) }
                                                />
                                                <label htmlFor={ `rewardedMarks${ i }` }>Rewarded Marks</label>
                                            </div>

                                            <div className='input-field col s12'>
                                                <i className='material-icons prefix'>insert_comment</i>
                                                <input id={ `comment${ i }` }
                                                       type='text'
                                                       className='validate'
                                                       value={ marks.comment }
                                                       onChange={ e => updateMarkEntry(i, 'comment', e.target.value) }
                                                />
                                                <label htmlFor={ `comment${ i }` }>Task Description</label>
                                            </div>

                                            <div className='col s12'>
                                                <button className='btn waves-effect waves-light'
                                                        onClick={ () => handleResetBtnClicked(i) }
                                                >
                                                    <i className="fas fa-eraser" />&nbsp;
                                                    Reset
                                                </button>

                                                {
                                                    marksToUpdate.markBreakdowns.length > 1 &&
                                                    <button className='btn waves-effect waves-light red right'
                                                            onClick={ () => handleRemoveEntryBtnClicked(i) }
                                                    >
                                                        <i className="fas fa-trash-alt" />&nbsp;
                                                        Remove this entry
                                                    </button>
                                                }
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            )
                        }
                    </div>

                    <div className='col s12'>
                        <button className='btn waves-effect waves-light'
                                onClick={ () => attemptUpdatingMarks() }
                        >
                            <i className="fas fa-check" />&nbsp;
                            Done - save marks
                        </button>

                        <button className='btn waves-effect waves-light green right'
                                onClick={ () => handleAddEntryBtnClicked() }
                        >
                            <i className="fas fa-plus-circle" />&nbsp;
                            Add new entry
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
)(ManageEnrolments);