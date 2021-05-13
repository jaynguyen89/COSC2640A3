import React from 'react';
import {connect} from 'react-redux';

import Spinner from "../../../shared/Spinner";
import Alert from "../../../shared/Alert";
import {ITeacherComponent} from "../redux/interfaces";
import {checkSession, EMPTY_STATUS, EMPTY_STRING, IStatusMessage} from "../../../providers/helpers";
import {invokeUpdateTeacherRequest} from "../redux/actions";
import {clearAuthUser} from "../../authentication/redux/actions";
import * as accountConstants from "../redux/constants";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    updateTeacher: state.accountStore.updateTeacher
});

const mapActionsToProps = {
    invokeUpdateTeacherRequest,
    clearAuthUser
};

const TeacherDetail = (props: ITeacherComponent) => {
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [teacherDetail, setTeacherDetail] = React.useState({
        id: EMPTY_STRING,
        company: EMPTY_STRING,
        jobTitle: EMPTY_STRING,
        personalWebsite: EMPTY_STRING
    });

    React.useEffect(() => {
        setTeacherDetail({
            id: props.teacherDetail.teacherId,
            company: props.teacherDetail.company,
            jobTitle: props.teacherDetail.jobTitle,
            personalWebsite: props.teacherDetail.personalWebsite
        });
    }, [props.teacherDetail]);

    const updateTeacherDetail = (field: string, value: string) => {
        if (field === 'company') setTeacherDetail({ ...teacherDetail, company: value });
        if (field === 'jobTitle') setTeacherDetail({ ...teacherDetail, jobTitle: value });
        if (field === 'personalWebsite') setTeacherDetail({ ...teacherDetail, personalWebsite: value });
    }

    React.useEffect(() => {
        if (props.updateTeacher.action === accountConstants.UPDATE_TEACHER_DETAIL_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.updateTeacher.error?.message);

        if (props.updateTeacher.action === accountConstants.UPDATE_TEACHER_DETAIL_REQUEST_SUCCESS)
            if (props.updateTeacher.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.updateTeacher.payload.result === 0)
                setStatusMessage({ messages: props.updateTeacher.payload.messages, type: 'error' } as IStatusMessage);
            else
                setStatusMessage({ messages: ['Your teacher details have been updated.'], type: 'success' } as IStatusMessage);
    }, [props.updateTeacher]);

    return (
        <div className='row'>
            { props.updateTeacher.action === accountConstants.UPDATE_TEACHER_DETAIL_REQUEST_SENT && <Spinner /> }
            <Alert { ...statusMessage } />

            <div className='col s6'>
                <div className='input-field col s12'>
                    <i className='material-icons prefix'>apartment</i>
                    <input id='company'
                           type='text'
                           className='validate'
                           value={ teacherDetail.company }
                           onChange={ e => updateTeacherDetail('company', e.target.value) }
                    />
                    <label htmlFor='company'>School Name</label>
                </div>
            </div>

            <div className='col s6'>
                <div className='input-field col s12'>
                    <i className='material-icons prefix'>work</i>
                    <input id='jobTitle'
                           type='text'
                           className='validate'
                           value={ teacherDetail.jobTitle }
                           onChange={ e => updateTeacherDetail('jobTitle', e.target.value) }
                    />
                    <label htmlFor='jobTitle'>Job Title</label>
                </div>
            </div>

            <div className='col s12'>
                <div className='input-field col s12'>
                    <i className='material-icons prefix'>language</i>
                    <input id='personalUrl'
                           type='text'
                           className='validate'
                           value={ teacherDetail.personalWebsite }
                           onChange={ e => updateTeacherDetail('personalWebsite', e.target.value) }
                    />
                    <label htmlFor='personalUrl'>Personal URL</label>
                </div>
            </div>

            <div className='col s12'>
                <button className='btn waves-effect waves-light'
                        onClick={ () => props.invokeUpdateTeacherRequest(props.authUser, teacherDetail) }
                >
                    <i className="fas fa-pen" />
                    &nbsp; Update
                </button>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(TeacherDetail);