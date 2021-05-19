import React from 'react';
import {connect} from 'react-redux';
import _ from 'lodash';
import $ from 'jquery';
import M from 'materialize-css';
import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import {clearAuthUser} from "../authentication/redux/actions";
import {IEnrolment, IEnrolmentList} from "./redux/interfaces";
import {invokeGetStudentEnrolmentsRequest, invokeUnenrolFromClassroomRequest} from "./redux/actions";
import * as studentConstants from "./redux/constants";
import {checkSession, checkUnenrolmentResult, EMPTY_STATUS, IStatusMessage} from "../../providers/helpers";
import EnrolmentInfo from "./EnrolmentInfo";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    getStudentEnrolments: state.studentStore.getStudentEnrolments,
    unenrolClassroom: state.studentStore.unenrolClassroom
});

const mapActionsToProps = {
    clearAuthUser,
    invokeGetStudentEnrolmentsRequest,
    invokeUnenrolFromClassroomRequest
};

const EnrolmentList = (props: IEnrolmentList) => {
    const [enrolments, setEnrolments] = React.useState(Array<IEnrolment>());
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);

    React.useEffect(() => {
        props.invokeGetStudentEnrolmentsRequest(props.authUser);
    }, []);

    React.useEffect(() => {
        if (props.getStudentEnrolments.action === studentConstants.GET_STUDENT_ENROLMENTS_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.getStudentEnrolments.error?.message);

        if (props.getStudentEnrolments.action === studentConstants.GET_STUDENT_ENROLMENTS_REQUEST_SUCCESS)
            if (props.getStudentEnrolments.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.getStudentEnrolments.payload.result === 0)
                setStatusMessage({ messages: props.getStudentEnrolments.payload.messages, type: 'error' } as IStatusMessage);
            else
                setEnrolments(props.getStudentEnrolments.payload.data as Array<IEnrolment>);
    }, [props.getStudentEnrolments]);

    React.useEffect(() => {
        const result = checkUnenrolmentResult(props.clearAuthUser, setStatusMessage, props.unenrolClassroom, _.cloneDeep(enrolments));
        if (result) setEnrolments(result);
    }, [props.unenrolClassroom]);

    React.useEffect(() => {
        M.Tabs.init($('.tabs'), { duration: 250, swipeable: false });
        M.Collapsible.init($('.collapsible'), { accordion: true });
    }, [enrolments]);

    return (
        <div className='row'>
            <div className='col s12'>
                <h5 className='title'>
                    <i className="fas fa-bookmark" />
                    &nbsp;My enrolments & invoices
                </h5>
            </div>
            {
                (
                    props.getStudentEnrolments.action === studentConstants.GET_STUDENT_ENROLMENTS_REQUEST_SENT ||
                    props.unenrolClassroom.action === studentConstants.UNENROL_INTO_CLASSROOM_REQUEST_SENT
                ) && <Spinner />
            }
            <Alert { ...statusMessage } closeAlert={ () => setStatusMessage(EMPTY_STATUS) } />

            <div className='col s12'>
                <div className='row'>
                    {
                        (
                            enrolments.length === 0 &&
                            <div className='row center-align'>You haven't enrolled into any classroom.</div>
                        ) ||
                        enrolments.map(enrolment =>
                            <div className='col l4 m6 s12' key={ enrolment.id }>
                                <EnrolmentInfo
                                    enrolment={ enrolment }
                                    handleUnenrolBtn={ () => props.invokeUnenrolFromClassroomRequest(props.authUser, enrolment.id) }
                                    viewByStudent
                                />
                            </div>
                        )
                    }
                </div>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(EnrolmentList);