import React from 'react';
import {connect} from 'react-redux';
import _ from 'lodash';
import $ from 'jquery';
import M from 'materialize-css';
import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import * as classroomConstants from './redux/constants';
import HeaderNav from "../../shared/HeaderNav";
import {IClassroom, IManageClassroom} from "./redux/interfaces";
import {
    invokeCompletedClassroomsRequest,
    invokeGetAllTeacherClassroomsRequest,
    invokeRemoveClassroomsRequest
} from "./redux/actions";
import {
    checkSession,
    EMPTY_STATUS,
    EMPTY_STRING,
    IStatusMessage,
    modalOptions, TASK_CREATE, TASK_UPDATE,
    TASK_VIEW
} from "../../providers/helpers";
import {clearAuthUser} from "../authentication/redux/actions";
import ClassroomCard from "./components/ClassroomCard";
import ClassroomModal from "./components/ClassroomModal";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    getTeacherClassrooms: state.classroomStore.getTeacherClassrooms,
    removeClassroom: state.classroomStore.removeClassroom,
    completedClassroom: state.classroomStore.completedClassroom
});

const mapActionsToProps = {
    clearAuthUser,
    invokeGetAllTeacherClassroomsRequest,
    invokeRemoveClassroomsRequest,
    invokeCompletedClassroomsRequest
};

const ManageClassrooms = (props: IManageClassroom) => {
    const [classrooms, setClassrooms] = React.useState(Array<IClassroom>());
    const [completedClassrooms, setCompletedClassrooms] = React.useState(Array<IClassroom>());
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [selectedClassroomId, setSelectedClassroomId] = React.useState(EMPTY_STRING);
    const [modalTask, setModalTask] = React.useState(TASK_VIEW);

    React.useEffect(() => {
        props.invokeGetAllTeacherClassroomsRequest(props.authUser, null);
        M.Modal.init($('.modal'), {
            ...modalOptions,
            onCloseEnd: () => {
                setSelectedClassroomId(EMPTY_STRING);
                setModalTask(TASK_VIEW);
            }
        });
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
                setClassrooms((props.getTeacherClassrooms.payload.data as any).classrooms as Array<IClassroom>);
                if ((props.getTeacherClassrooms.payload.data as any).completedClassrooms)
                    setCompletedClassrooms((props.getTeacherClassrooms.payload.data as any).completedClassrooms);
            }
    }, [props.getTeacherClassrooms]);

    const handleAddClassroomBtnClicked = () => {
        setSelectedClassroomId(EMPTY_STRING);
        setModalTask(TASK_CREATE);
        M.Modal.getInstance(document.querySelector('.modal') as Element).open();
    }

    const handleClassNameClicked = (classroomId: string) => {
        setSelectedClassroomId(classroomId);
        setModalTask(TASK_VIEW);
        M.Modal.getInstance(document.querySelector('.modal') as Element).open();
    }

    const handleReviseBtnCLicked = (classroomId: string) => {
        setSelectedClassroomId(classroomId);
        setModalTask(TASK_UPDATE);
        M.Modal.getInstance(document.querySelector('.modal') as Element).open();
    }

    const handleRemoveBtnClicked = (classroomId: string) => {
        setSelectedClassroomId(classroomId);
        props.invokeRemoveClassroomsRequest(props.authUser, classroomId);
    }

    const handleMarkAsCompletedBtnClicked = (classroomId: string) => {
        setSelectedClassroomId(classroomId);
        props.invokeCompletedClassroomsRequest(props.authUser, classroomId);
    }

    const attemptCreateClassroom = (newClassroom: IClassroom) => {
        M.Modal.getInstance(document.querySelector('.modal') as Element).close();
    }

    const attemptUpdateClassroom = (updatedClassroom: IClassroom) => {
        M.Modal.getInstance(document.querySelector('.modal') as Element).close();
    }

    React.useEffect(() => {
        if (props.removeClassroom.action === classroomConstants.REMOVE_CLASSROOMS_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.removeClassroom.error?.message);

        if (props.removeClassroom.action === classroomConstants.REMOVE_CLASSROOMS_REQUEST_SUCCESS)
            if (props.removeClassroom.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.removeClassroom.payload.result === 0)
                setStatusMessage({ messages: props.removeClassroom.payload.messages, type: 'error' } as IStatusMessage);
            else {
                let clone = _.cloneDeep(classrooms);
                const removedClassroom = clone.find(classroom => classroom.id === selectedClassroomId);

                const refinedClassrooms = clone.splice(clone.indexOf(removedClassroom as IClassroom), 1);
                setClassrooms(refinedClassrooms);

                setSelectedClassroomId(EMPTY_STRING);
                setStatusMessage({ messages: ['The classroom has been deleted.'], type: 'success' } as IStatusMessage);
            }
    }, [props.removeClassroom]);

    React.useEffect(() => {
        if (props.completedClassroom.action === classroomConstants.COMPLETED_CLASSROOMS_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.completedClassroom.error?.message);

        if (props.completedClassroom.action === classroomConstants.COMPLETED_CLASSROOMS_REQUEST_SUCCESS)
            if (props.completedClassroom.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.completedClassroom.payload.result === 0)
                setStatusMessage({ messages: props.completedClassroom.payload.messages, type: 'error' } as IStatusMessage);
            else {
                let classroomClone = _.cloneDeep(classrooms);
                const completedClassroom = classroomClone.find(classroom => classroom.id === selectedClassroomId);

                const refinedClassrooms = classroomClone.splice(classroomClone.indexOf(completedClassroom as IClassroom), 1);
                setClassrooms(refinedClassrooms);

                let completedClone = _.cloneDeep(completedClassrooms);
                completedClone.push(completedClassroom as IClassroom);
                setCompletedClassrooms(completedClone);

                setSelectedClassroomId(EMPTY_STRING);
                setStatusMessage({ messages: ['The classroom has been marked as completed.'], type: 'success' } as IStatusMessage);
            }
    }, [props.completedClassroom]);

    return (
        <div className='container' style={{ marginTop: '2.5em' }}>
            <HeaderNav
                location='manage-classrooms'
                greetingName={ localStorage.getItem('preferredName') as string }
            />

            <div className='card'>
                <div className='card-content'>
                    <div className='row'>
                        {
                            (
                                props.removeClassroom.action === classroomConstants.REMOVE_CLASSROOMS_REQUEST_SENT ||
                                props.completedClassroom.action === classroomConstants.COMPLETED_CLASSROOMS_REQUEST_SENT
                            ) && <Spinner />
                        }

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
                        <Alert { ...statusMessage } />

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
                                        handleTitleClicked={ handleClassNameClicked }
                                        handleReviseBtn={ handleReviseBtnCLicked }
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
                        <Alert { ...statusMessage } />

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
                selectedClassroomId={ selectedClassroomId }
                task={ modalTask }
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