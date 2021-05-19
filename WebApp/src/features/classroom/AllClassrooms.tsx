import React from 'react';
import {connect} from 'react-redux';
import $ from 'jquery';
import M from 'materialize-css';
import moment from 'moment';
import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import HeaderNav from "../../shared/HeaderNav";
import {defaultClassroom, IAllClassrooms, IClassroomData} from "./redux/interfaces";
import {clearAuthUser} from "../authentication/redux/actions";
import {invokeGetAllClassroomsRequest, invokeGetClassroomDetailRequest} from "./redux/actions";
import {
    checkSession,
    EMPTY_STATUS,
    EMPTY_STRING,
    IStatusMessage,
    modalOptions, removeGlobalMessage, setGlobalMessage
} from "../../providers/helpers";
import * as classroomConstants from './redux/constants';
import * as studentConstants from '../student/redux/constants';
import {invokeStudentEnrolmentRequest} from "../student/redux/actions";
import ClassroomInfo from "./components/ClassroomInfo";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    getAllClassrooms: state.classroomStore.getAllClassrooms,
    getClassroomDetail: state.classroomStore.getClassroomDetail,
    enrolClassroom: state.studentStore.enrolClassroom
});

const mapActionsToProps = {
    clearAuthUser,
    invokeGetAllClassroomsRequest,
    invokeGetClassroomDetailRequest,
    invokeStudentEnrolmentRequest
};

const AllClassrooms = (props: IAllClassrooms) => {
    const [classrooms, setClassrooms] = React.useState(Array<IClassroomData>());
    const [selectedClassroomId, setSelectedClassroomId] = React.useState(EMPTY_STRING);
    const [classroomDetails, setClassroomDetails] = React.useState(defaultClassroom);
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [shouldEnableButtons, setShouldEnableButtons] = React.useState(true);

    React.useEffect(() => {
        const storedGlobalMessage = sessionStorage.getItem('globalMessage');
        if (storedGlobalMessage) {
            setStatusMessage(JSON.parse(storedGlobalMessage) as IStatusMessage);
            removeGlobalMessage();
        }

        M.Modal.init($('.modal'), {
            ...modalOptions,
            onCloseStart: () => {
                setShouldEnableButtons(true);
                setSelectedClassroomId(EMPTY_STRING);
                setClassroomDetails(defaultClassroom);
            }
        });

        props.invokeGetAllClassroomsRequest(props.authUser);
    }, []);

    React.useEffect(() => {
        if (props.getAllClassrooms.action === classroomConstants.GET_ALL_CLASSROOMS_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.getAllClassrooms.error?.message);

        if (props.getAllClassrooms.action === classroomConstants.GET_ALL_CLASSROOMS_REQUEST_SUCCESS)
            if (props.getAllClassrooms.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.getAllClassrooms.payload.result === 0)
                setStatusMessage({ messages: props.getAllClassrooms.payload.messages, type: 'error' } as IStatusMessage);
            else
                setClassrooms(props.getAllClassrooms.payload.data as Array<IClassroomData>);
    }, [props.getAllClassrooms]);

    const handleViewingDetails = (classroomId: string) => {
        setShouldEnableButtons(false);
        setSelectedClassroomId(classroomId);
        props.invokeGetClassroomDetailRequest(props.authUser, classroomId);
    }

    const attemptEnrolment = (classroomId: string) => {
        setSelectedClassroomId(classroomId);
        props.invokeStudentEnrolmentRequest(props.authUser, classroomId);
    }

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
                setClassroomDetails(props.getClassroomDetail.payload.data as IClassroomData);
                M.Modal.getInstance(document.querySelector('#classroomDetailModal') as Element).open();
            }
    }, [props.getClassroomDetail]);

    React.useEffect(() => {
        setSelectedClassroomId(EMPTY_STRING);

        if (props.enrolClassroom.action === studentConstants.ENROL_INTO_CLASSROOM_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.enrolClassroom.error?.message);

        if (props.enrolClassroom.action === studentConstants.ENROL_INTO_CLASSROOM_REQUEST_SUCCESS)
            if (props.enrolClassroom.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.enrolClassroom.payload.result === 0)
                setStatusMessage({ messages: props.enrolClassroom.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setStatusMessage(EMPTY_STATUS);
                setGlobalMessage({ messages: ['You have successfully enrolled into the classroom, view it in your home page.'], type: 'error' } as IStatusMessage);
                props.invokeGetAllClassroomsRequest(props.authUser);
            }
    }, [props.enrolClassroom]);

    return (
        <div className='container' style={{ marginTop: '2.5em' }}>
            <HeaderNav
                location='home'
                greetingName={ localStorage.getItem('preferredName') as string }
            />
            { props.getAllClassrooms.action === classroomConstants.GET_ALL_CLASSROOMS_REQUEST_SENT && <Spinner /> }
            <Alert { ...statusMessage } closeAlert={ () => setStatusMessage(EMPTY_STATUS) } />

            <div className='row'>
                <div className='col s12 center-align'>
                    <h4 style={{ marginTop: 0 }}>Browsing Classrooms</h4>
                    <hr />
                </div>

                {
                    classrooms.map(classroom =>
                        <div className='col m4 s12' key={ classroom.id }>
                            <div className='card'>
                                <div className='row card-content'>
                                    {
                                        (
                                            props.getClassroomDetail.action === classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_SENT ||
                                            props.enrolClassroom.action === studentConstants.ENROL_INTO_CLASSROOM_REQUEST_SENT
                                        ) &&
                                        selectedClassroomId === classroom.id &&
                                        <div className='corner'><Spinner /></div>
                                    }

                                    <a className='section-header text-link'
                                       onClick={ () => handleViewingDetails(classroom.id) }
                                    >
                                        <i className="fas fa-link" />
                                        &nbsp;{ classroom.className }
                                    </a>


                                    <div className='col s12 small-text'>
                                        <span>Teacher:</span>
                                        <span className='right'>{ classroom.teacherName }</span>
                                    </div>

                                    <div className='col s12 small-text'>
                                        <span>Price:</span>
                                        <span className='right'>${ classroom.price }</span>
                                    </div>

                                    <div className='col s12 small-text' style={{ marginTop: '1em' }}>
                                        <button className={ (shouldEnableButtons && 'btn waves-effect waves-light') || 'btn disabled' }
                                                onClick={ () => handleViewingDetails(classroom.id) }
                                        >
                                            <i className="fas fa-align-center" />&nbsp;
                                            Details
                                        </button>

                                        <button className={ (shouldEnableButtons && 'btn waves-effect waves-light orange right') || 'btn disabled right' }
                                                onClick={ () => attemptEnrolment(classroom.id) }
                                        >
                                            <i className="fas fa-sign-in-alt" />&nbsp;
                                            Enrol
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    )
                }
            </div>

            <div className='modal' id='classroomDetailModal'>
                <ClassroomInfo classroom={ classroomDetails } showTitle />
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(AllClassrooms);