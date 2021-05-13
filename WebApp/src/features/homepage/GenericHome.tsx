import React from 'react';
import {connect} from 'react-redux';
import M from 'materialize-css';
import {
    invokeGetStudentDetailRequest,
    invokeGetTeacherDetailRequest
} from "./redux/actions";
import StudentDetail from "./components/StudentDetail";
import TeacherDetail from "./components/TeacherDetail";
import {defaultStudent, defaultTeacher, IGenericHome, IStudentDetail, ITeacherDetail} from "./redux/interfaces";
import * as accountConstants from './redux/constants';
import * as authenticationConstants from '../authentication/redux/constants';
import Spinner from "../../shared/Spinner";
import {checkSession, EMPTY_STATUS, IStatusMessage, setGlobalMessage} from "../../providers/helpers";
import {
    clearAuthUser,
    invokeSwitchRoleRequest,
    loadAuthUser
} from "../authentication/redux/actions";
import Alert from "../../shared/Alert";
import AccountDetail from "./components/AccountDetail";
import TwoFactorDetail from "./components/TwoFactorDetail";
import HeaderNav from "../../shared/HeaderNav";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    getStudentDetail: state.accountStore.getStudentDetail,
    getTeacherDetail: state.accountStore.getTeacherDetail,
    switchRole: state.authenticationStore.switchRole
});

const mapActionToProps = {
    invokeGetStudentDetailRequest,
    invokeGetTeacherDetailRequest,
    clearAuthUser,
    invokeSwitchRoleRequest,
    loadAuthUser
};

const GenericHome = (props: IGenericHome) => {
    const [studentDetail, setStudentDetail] = React.useState(defaultStudent);
    const [teacherDetail, setTeacherDetail] = React.useState(defaultTeacher);
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [shouldShowPage, setShouldShowPage] = React.useState(false);

    React.useEffect(() => {
        if (props.authUser.role === 0) props.invokeGetStudentDetailRequest(props.authUser);
        else props.invokeGetTeacherDetailRequest(props.authUser);
    }, [props.authUser.role]);

    React.useEffect(() => {
        if (props.switchRole.action === authenticationConstants.SWITCH_ROLE_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.switchRole.error?.message);

        if (props.switchRole.action === authenticationConstants.SWITCH_ROLE_REQUEST_SUCCESS)
            if (props.switchRole.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.switchRole.payload.result === 0)
                setStatusMessage({ messages: props.switchRole.payload.messages, type: 'error' } as IStatusMessage);
            else {
                localStorage.setItem('preferredName', (props.authUser.role === 0 && studentDetail.preferredName) || teacherDetail.preferredName);
                localStorage.setItem('role', props.switchRole.payload.data as unknown as string);
                props.loadAuthUser();
                alert(`You are now logged in as ${ (Number(props.switchRole.payload.data) === 0 && 'Student') || 'Teacher' }`);
            }
    }, [props.switchRole]);

    React.useEffect(() => {
        if (props.getStudentDetail.action === accountConstants.GET_STUDENT_DETAIL_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.getStudentDetail.error?.message);

        if (props.getStudentDetail.action === accountConstants.GET_STUDENT_DETAIL_REQUEST_SUCCESS)
            if (props.getStudentDetail.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.getStudentDetail.payload.result === 0)
                setStatusMessage({ messages: props.getStudentDetail.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setStudentDetail(props.getStudentDetail.payload.data as IStudentDetail);
                setShouldShowPage(true);
                return;
            }

        setShouldShowPage(false);
    }, [props.getStudentDetail]);

    React.useEffect(() => {
        if (props.getTeacherDetail.action === accountConstants.GET_TEACHER_DETAIL_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.getTeacherDetail.error?.message);

        if (props.getTeacherDetail.action === accountConstants.GET_TEACHER_DETAIL_REQUEST_SUCCESS)
            if (props.getTeacherDetail.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.getTeacherDetail.payload.result === 0)
                setStatusMessage({ messages: props.getTeacherDetail.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setTeacherDetail(props.getTeacherDetail.payload.data as ITeacherDetail);
                setShouldShowPage(true);
                return;
            }

        setShouldShowPage(false);
    }, [props.getTeacherDetail]);

    if (!props.authUser.isAuthenticated)
        window.location.href = '/';

    return (
        <div className='container' style={{ marginTop: '2.5em' }}>
            {
                (
                    props.getStudentDetail.action === accountConstants.GET_STUDENT_DETAIL_REQUEST_SENT ||
                    props.getTeacherDetail.action === accountConstants.GET_TEACHER_DETAIL_REQUEST_SENT
                ) &&
                <div style={{ paddingTop: '3em' }}>
                    <Spinner />
                </div>
            }
            <Alert { ...statusMessage } />

            {
                shouldShowPage &&
                <div className='row'>
                    <div className='col s12'>
                        <HeaderNav
                            location='home'
                            greetingName={ (props.authUser.role === 0 && studentDetail.preferredName) || teacherDetail.preferredName }
                        />

                        <div className='clearfix'/>
                        <div className='card'>
                            <div className='card-content'>
                                <div className='row'>
                                    <div className='col s12'>
                                        { props.switchRole.action === authenticationConstants.SWITCH_ROLE_REQUEST_SENT && <Spinner /> }

                                        You are signed in as: <span style={{ color: '#26a69a' }}>{ (props.authUser.role === 0 && 'Student') || 'Teacher' }</span>
                                        <button className='btn waves-effect waves-light'
                                                style={{ marginLeft: '1em' }}
                                                onClick={ () => props.invokeSwitchRoleRequest(props.authUser) }
                                        >
                                            Switch to { (props.authUser.role === 0 && 'Teacher') || 'Student' }
                                        </button>
                                    </div>
                                </div>

                                <h5 className='title'>
                                    <i className="fas fa-user-graduate"/>
                                    &nbsp;{ (props.authUser.role === 0 && 'Student') || 'Teacher' } Details
                                </h5>

                                {
                                    (
                                        props.authUser.role === 0 &&
                                        <StudentDetail studentDetail={ studentDetail } />
                                    ) ||
                                    <TeacherDetail teacherDetail={ teacherDetail } />
                                }
                            </div>

                            <div className='card-content'>
                                <h5 className='title'>
                                    <i className="fas fa-user-alt" />
                                    &nbsp;Account Details
                                </h5>

                                <AccountDetail accountDetail={ (props.authUser.role === 0 && studentDetail) || teacherDetail } />
                                <TwoFactorDetail accountDetail={ (props.authUser.role === 0 && studentDetail) || teacherDetail } />
                            </div>
                        </div>
                    </div>
                </div>
            }
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionToProps
)(GenericHome);