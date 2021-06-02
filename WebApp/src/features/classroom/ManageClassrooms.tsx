import React from 'react';
import {connect} from 'react-redux';
import $ from 'jquery';
import M from 'materialize-css';
import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import * as classroomConstants from './redux/constants';
import HeaderNav from "../../shared/HeaderNav";
import {defaultClassroom, getClassroom, IClassroomData, IManageClassroom} from "./redux/interfaces";
import {
    invokeCompletedClassroomsRequest, invokeCreateClassroomsRequest,
    invokeGetAllTeacherClassroomsRequest, invokeGetClassroomDetailRequest,
    invokeRemoveClassroomsRequest, invokeUpdateClassroomsRequest
} from "./redux/actions";
import {
    checkSession,
    EMPTY_STATUS,
    EMPTY_STRING,
    IStatusMessage,
    modalOptions, removeGlobalMessage, setGlobalMessage, TASK_CREATE, TASK_UPDATE,
    TASK_VIEW
} from "../../providers/helpers";
import {clearAuthUser} from "../authentication/redux/actions";
import ClassroomCard from "./components/ClassroomCard";
import ClassroomModal from "./components/ClassroomModal";
import moment from "moment";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    getTeacherClassrooms: state.classroomStore.getTeacherClassrooms,
    removeClassroom: state.classroomStore.removeClassroom,
    completedClassroom: state.classroomStore.completedClassroom,
    createClassroom: state.classroomStore.createClassroom,
    updateClassroom: state.classroomStore.updateClassroom,
    getClassroomDetail: state.classroomStore.getClassroomDetail
});

const mapActionsToProps = {
    clearAuthUser,
    invokeGetAllTeacherClassroomsRequest,
    invokeRemoveClassroomsRequest,
    invokeCompletedClassroomsRequest,
    invokeCreateClassroomsRequest,
    invokeUpdateClassroomsRequest,
    invokeGetClassroomDetailRequest
};

const ManageClassrooms = (props: IManageClassroom) => {
    const [classrooms, setClassrooms] = React.useState(Array<IClassroomData>());
    const [completedClassrooms, setCompletedClassrooms] = React.useState(Array<IClassroomData>());
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [selectedClassroomId, setSelectedClassroomId] = React.useState(EMPTY_STRING);
    const [selectedClassroom, setSelectedClassroom] = React.useState(defaultClassroom);
    const [modalTask, setModalTask] = React.useState(TASK_VIEW);
    const [isTaskRunning, setIsTaskRunning] = React.useState(false);
    const [selectedDate, setSelectedDate] = React.useState(EMPTY_STRING);
    const [selectedTime, setSelectedTime] = React.useState(EMPTY_STRING);

    React.useEffect(() => {
        props.invokeGetAllTeacherClassroomsRequest(props.authUser, null);
        M.Modal.init($('.modal'), {
            ...modalOptions,
            onCloseStart: () => {
                setSelectedClassroomId(EMPTY_STRING);
                setSelectedClassroom(defaultClassroom);
                setIsTaskRunning(false);
                setModalTask(TASK_VIEW);
                setStatusMessage(EMPTY_STATUS);
                setSelectedDate(EMPTY_STRING);
                setSelectedTime(EMPTY_STRING);
            }
        });
    }, []);

    React.useEffect(() => {
        const storedGlobalMessage = sessionStorage.getItem('globalMessage');
        if (storedGlobalMessage) {
            setStatusMessage(JSON.parse(storedGlobalMessage) as IStatusMessage);
            removeGlobalMessage();
        }

        if (props.getTeacherClassrooms.action === classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.getTeacherClassrooms.error?.message);

        if (props.getTeacherClassrooms.action === classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_SUCCESS)
            if (props.getTeacherClassrooms.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.getTeacherClassrooms.payload.result === 0)
                setStatusMessage({ messages: props.getTeacherClassrooms.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setClassrooms((props.getTeacherClassrooms.payload.data as any).classrooms as Array<IClassroomData>);
                if ((props.getTeacherClassrooms.payload.data as any).completedClassrooms)
                    setCompletedClassrooms((props.getTeacherClassrooms.payload.data as any).completedClassrooms);
            }
    }, [props.getTeacherClassrooms]);

    const handleAddClassroomBtnClicked = () => {
        setSelectedClassroomId(EMPTY_STRING);
        setSelectedClassroom(defaultClassroom);
        setModalTask(TASK_CREATE);
        M.Modal.getInstance(document.querySelector('.modal') as Element).open();
    }

    const handleClassNameClicked = (classroomId: string) => {
        setSelectedClassroomId(classroomId);
        props.invokeGetClassroomDetailRequest(props.authUser, classroomId);
        setModalTask(TASK_VIEW);
    }

    const handleReviseBtnClicked = (classroomId: string) => {
        setSelectedClassroomId(classroomId);
        props.invokeGetClassroomDetailRequest(props.authUser, classroomId);
        setModalTask(TASK_UPDATE);
    }

    const handleRemoveBtnClicked = (classroomId: string) => {
        setSelectedClassroomId(classroomId);
        props.invokeRemoveClassroomsRequest(props.authUser, classroomId);
    }

    const handleMarkAsCompletedBtnClicked = (classroomId: string) => {
        setSelectedClassroomId(classroomId);
        props.invokeCompletedClassroomsRequest(props.authUser, classroomId);
    }

    const handleDateTimeSelection = (dt: string, field: string) => {
        if (field === 'date') setSelectedDate(dt);
        if (field === 'time') setSelectedTime(dt);
    }

    const attemptCreateClassroom = (newClassroom: IClassroomData) => {
        if (selectedDate === EMPTY_STRING) {
            alert('Please select a Commence Date for this classroom.');
            return;
        }

        const classroom = getClassroom(newClassroom);
        classroom.commencedOn = moment(`${ selectedDate } ${ selectedTime }`.trim()).format();

        setIsTaskRunning(true);
        setSelectedClassroom(newClassroom);
        props.invokeCreateClassroomsRequest(props.authUser, classroom);
    }

    const attemptUpdateClassroom = (updatedClassroom: IClassroomData) => {
        const classroom = getClassroom(updatedClassroom);
        classroom.id = updatedClassroom.id;

        if (selectedDate !== EMPTY_STRING) classroom.commencedOn = moment(`${ selectedDate } ${ selectedTime }`.trim()).format();

        setIsTaskRunning(true);
        setSelectedClassroom(updatedClassroom);
        props.invokeUpdateClassroomsRequest(props.authUser, classroom);
    }

    React.useEffect(() => {
        if (props.removeClassroom.action === classroomConstants.REMOVE_CLASSROOM_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.removeClassroom.error?.message);

        if (props.removeClassroom.action === classroomConstants.REMOVE_CLASSROOM_REQUEST_SUCCESS)
            if (props.removeClassroom.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.removeClassroom.payload.result === 0)
                setStatusMessage({ messages: props.removeClassroom.payload.messages, type: 'error' } as IStatusMessage);
            else {
                props.invokeGetAllTeacherClassroomsRequest(props.authUser, null);
                setGlobalMessage({ messages: ['The classroom has been deleted.'], type: 'success' } as IStatusMessage);
            }

        setSelectedClassroomId(EMPTY_STRING);
    }, [props.removeClassroom]);

    React.useEffect(() => {
        if (props.completedClassroom.action === classroomConstants.COMPLETED_CLASSROOM_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.completedClassroom.error?.message);

        if (props.completedClassroom.action === classroomConstants.COMPLETED_CLASSROOM_REQUEST_SUCCESS)
            if (props.completedClassroom.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.completedClassroom.payload.result === 0)
                setStatusMessage({ messages: props.completedClassroom.payload.messages, type: 'error' } as IStatusMessage);
            else {
                props.invokeGetAllTeacherClassroomsRequest(props.authUser, null);
                setGlobalMessage({ messages: ['The classroom has been marked as completed and moved to Completed Classrooms.'], type: 'success' } as IStatusMessage);
            }

        setSelectedClassroomId(EMPTY_STRING);
    }, [props.completedClassroom]);

    React.useEffect(() => {
        setSelectedClassroomId(EMPTY_STRING);

        if (props.getClassroomDetail.action === classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_SENT)
            checkSession(props.clearAuthUser, setStatusMessage, props.getClassroomDetail.error?.message);

        if (props.getClassroomDetail.action === classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_SUCCESS)
            if (props.getClassroomDetail.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.getClassroomDetail.payload.result === 0)
                setStatusMessage({ messages: props.getClassroomDetail.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setStatusMessage(EMPTY_STATUS);
                setSelectedClassroom(props.getClassroomDetail.payload.data as IClassroomData);
                M.Modal.getInstance(document.querySelector('.modal') as Element).open();
            }
    }, [props.getClassroomDetail]);

    React.useEffect(() => {
        setIsTaskRunning(false);

        if (props.createClassroom.action === classroomConstants.CREATE_CLASSROOM_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.createClassroom.error?.message);

        if (props.createClassroom.action === classroomConstants.CREATE_CLASSROOM_REQUEST_SUCCESS)
            if (props.createClassroom.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.createClassroom.payload.result === 0)
                setStatusMessage({ messages: props.createClassroom.payload.messages, type: 'error' } as IStatusMessage);
            else {
                M.Modal.getInstance(document.querySelector('.modal') as Element).close();
                props.invokeGetAllTeacherClassroomsRequest(props.authUser, null);
                setGlobalMessage({ messages: ['Your classroom has been added successfully.'], type: 'success' } as IStatusMessage);
            }
    }, [props.createClassroom]);

    React.useEffect(() => {
        setIsTaskRunning(false);

        if (props.updateClassroom.action === classroomConstants.UPDATE_CLASSROOM_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.createClassroom.error?.message);

        if (props.updateClassroom.action === classroomConstants.UPDATE_CLASSROOM_REQUEST_SUCCESS)
            if (props.updateClassroom.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.updateClassroom.payload.result === 0)
                setStatusMessage({ messages: props.updateClassroom.payload.messages, type: 'error' } as IStatusMessage);
            else {
                M.Modal.getInstance(document.querySelector('.modal') as Element).close();
                props.invokeGetAllTeacherClassroomsRequest(props.authUser, null);
                setGlobalMessage({ messages: ['Your classroom has been updated successfully.'], type: 'success' } as IStatusMessage);
            }
    }, [props.updateClassroom]);

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
                                <i className="fab fa-buromobelexperte" />
                                &nbsp; Your classrooms
                            </p>

                            <button className='btn waves-effect waves-light right'
                                    onClick={ () => handleAddClassroomBtnClicked() }
                            >
                                <i className="fas fa-plus-circle" />
                                &nbsp; Add classroom
                            </button>
                        </div>
                    </div>

                    <div className='row'>
                        { props.getTeacherClassrooms.action === classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_SENT && <Spinner /> }
                        <Alert { ...statusMessage } closeAlert={ () => setStatusMessage(EMPTY_STATUS) } />

                        {
                            (
                                classrooms && classrooms.length === 0 &&
                                <div className='center-align'>
                                    <p>You don't have any classrooms.</p>
                                    <p>Please get started by adding or importing your first classrooms.</p>
                                </div>
                            ) ||
                            classrooms.map(classroom =>
                                <div className='col m4 s12' key={ classroom.id }>
                                    <ClassroomCard
                                        classroom={ classroom }
                                        selectedClassroomId={ selectedClassroomId }
                                        handleTitleClicked={ handleClassNameClicked }
                                        handleReviseBtn={ handleReviseBtnClicked }
                                        handleRemoveBtn={ handleRemoveBtnClicked }
                                        handleMarkAsCompletedBtn={ handleMarkAsCompletedBtnClicked }
                                    />
                                </div>
                            )
                        }
                    </div>
                </div>
            </div>

            <div className='card'>
                <div className='card-content'>
                    <div className='row'>
                        <p className='card-title left'>
                            <i className="fab fa-buromobelexperte" />
                            &nbsp; Completed classrooms
                        </p>
                    </div>

                    <div className='row'>
                        { props.getTeacherClassrooms.action === classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_SENT && <Spinner /> }

                        {
                            (
                                completedClassrooms.length === 0 &&
                                <div className='center-align'>
                                    <p>You don't have any completed classrooms.</p>
                                </div>
                            ) ||
                            completedClassrooms.map(classroom =>
                                <div className='col m4 s12' key={ classroom.id }>
                                    <ClassroomCard
                                        selectedClassroomId={ selectedClassroomId }
                                        classroom={ classroom } completed
                                        handleTitleClicked={ handleClassNameClicked }
                                    />
                                </div>
                            )
                        }
                    </div>
                </div>
            </div>

            <ClassroomModal
                selectedClassroom={ selectedClassroom }
                statusMessage={ statusMessage }
                closeAlert={ () => setStatusMessage(EMPTY_STATUS) }
                task={ modalTask }
                isTaskRunning={ isTaskRunning }
                datetime={{
                    date: selectedDate, time: selectedTime,
                    setDateTime: handleDateTimeSelection
                }}
                handleCreateBtn={ attemptCreateClassroom }
                handleUpdateBtn={ attemptUpdateClassroom }
            />
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ManageClassrooms);