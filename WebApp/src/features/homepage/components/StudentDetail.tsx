import React from 'react';
import {connect} from 'react-redux';

import Spinner from "../../../shared/Spinner";
import Alert from "../../../shared/Alert";
import {IStudentComponent} from "../redux/interfaces";
import {checkSession, EMPTY_STATUS, EMPTY_STRING, IStatusMessage} from "../../../providers/helpers";
import {invokeUpdateStudentRequest} from "../redux/actions";
import * as accountConstants from "../redux/constants";
import {clearAuthUser} from "../../authentication/redux/actions";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    updateStudent: state.accountStore.updateStudent
});

const mapActionsToProps = {
    invokeUpdateStudentRequest,
    clearAuthUser
};

const StudentDetail = (props: IStudentComponent) => {
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [studentDetail, setStudentDetail] = React.useState({
        id: EMPTY_STRING,
        schoolName: EMPTY_STRING,
        faculty: EMPTY_STRING,
        personalUrl: EMPTY_STRING
    });

    React.useEffect(() => {
        setStudentDetail({
            id: props.studentDetail.studentId,
            schoolName: props.studentDetail.schoolName,
            faculty: props.studentDetail.faculty,
            personalUrl: props.studentDetail.personalUrl
        });
    }, [props.studentDetail]);

    const updateStudentDetail = (field: string, value: string) => {
        if (field === 'schoolName') setStudentDetail({ ...studentDetail, schoolName: value });
        if (field === 'faculty') setStudentDetail({ ...studentDetail, faculty: value });
        if (field === 'personalUrl') setStudentDetail({ ...studentDetail, personalUrl: value });
    }

    React.useEffect(() => {
        if (props.updateStudent.action === accountConstants.UPDATE_STUDENT_DETAIL_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.updateStudent.error?.message);

        if (props.updateStudent.action === accountConstants.UPDATE_STUDENT_DETAIL_REQUEST_SUCCESS)
            if (props.updateStudent.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.updateStudent.payload.result === 0)
                setStatusMessage({ messages: props.updateStudent.payload.messages, type: 'error' } as IStatusMessage);
            else
                setStatusMessage({ messages: ['Your student details have been updated.'], type: 'success' } as IStatusMessage);
    }, [props.updateStudent]);

    return (
        <div className='row'>
            { props.updateStudent.action === accountConstants.UPDATE_STUDENT_DETAIL_REQUEST_SENT && <Spinner /> }
            <Alert { ...statusMessage } />

            <div className='col s6'>
                <div className='input-field col s12'>
                    <i className='material-icons prefix'>apartment</i>
                    <input id='school'
                           type='text'
                           className='validate'
                           value={ studentDetail.schoolName }
                           onChange={ e => updateStudentDetail('schoolName', e.target.value) }
                    />
                    <label htmlFor='school'>School Name</label>
                </div>
            </div>

            <div className='col s6'>
                <div className='input-field col s12'>
                    <i className='material-icons prefix'>school</i>
                    <input id='faculty'
                           type='text'
                           className='validate'
                           value={ studentDetail.faculty }
                           onChange={ e => updateStudentDetail('faculty', e.target.value) }
                    />
                    <label htmlFor='faculty'>Faculty</label>
                </div>
            </div>

            <div className='col s12'>
                <div className='input-field col s12'>
                    <i className='material-icons prefix'>language</i>
                    <input id='personalUrl'
                           type='text'
                           className='validate'
                           value={ studentDetail.personalUrl }
                           onChange={ e => updateStudentDetail('personalUrl', e.target.value) }
                    />
                    <label htmlFor='personalUrl'>Personal URL</label>
                </div>
            </div>

            <div className='col s12'>
                <button className='btn waves-effect waves-light'
                        onClick={ () => props.invokeUpdateStudentRequest(props.authUser, studentDetail) }
                >
                    Update
                </button>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(StudentDetail);