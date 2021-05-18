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
import {checkSession, EMPTY_STATUS, EMPTY_STRING, IStatusMessage} from "../../providers/helpers";
import moment from "moment";

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
        if (props.unenrolClassroom.action === studentConstants.UNENROL_INTO_CLASSROOM_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.unenrolClassroom.error?.message);

        if (props.unenrolClassroom.action === studentConstants.UNENROL_INTO_CLASSROOM_REQUEST_SUCCESS)
            if (props.unenrolClassroom.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.unenrolClassroom.payload.result === 0)
                setStatusMessage({ messages: props.unenrolClassroom.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setStatusMessage({ messages: ['Your enrolment and its corresponding invoice has been removed.'], type: 'error' } as IStatusMessage);
                const removedEnrolmentId = props.unenrolClassroom.payload.data as unknown as string;

                let clone = _.cloneDeep(enrolments);
                _.remove(clone, enrolment => enrolment.id === removedEnrolmentId);
                setEnrolments(clone);
            }
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
                            <div className='col m4 s12' key={ enrolment.id }>
                                <div className='card'>
                                    <div className='card-content'>
                                        {
                                            (
                                                enrolment.invoice.isPaid &&
                                                <>
                                                    <i className="fas fa-check-circle green-text" />&nbsp;Invoice Paid. Thank you!
                                                </>
                                            ) ||
                                            <>
                                                <i className="fas fa-info-circle red-text" />&nbsp;Invoice: ${ enrolment.invoice.amount }
                                                <button className='btn waves-effect waves-light right'
                                                        onClick={ () => alert('Open checkout page to select payment method then make payment.') }
                                                >
                                                    <i className="fas fa-shopping-cart" />&nbsp;Pay
                                                </button>
                                            </>
                                        }
                                    </div>

                                    <div className="card-tabs">
                                        <ul className="tabs tabs-fixed-width teal lighten-5">
                                            <li className="tab"><a href="#classroom">Classroom</a></li>
                                            <li className="tab"><a href="#marks">Marks</a></li>
                                            <li className="tab"><a href="#invoice">Invoice</a></li>
                                        </ul>
                                    </div>

                                    <div className="card-content grey lighten-4 small-text">
                                        <div id="classroom" className='row' style={{ marginBottom: 0 }}>
                                            <div className='col s12'>
                                                <span>Class Name:</span>
                                                <span className='right'>{ enrolment.classroom.className }</span>
                                            </div>

                                            <div className='col s12'>
                                                <span>Teacher:</span>
                                                <span className='right'>{ enrolment.classroom.teacherName }</span>
                                            </div>

                                            <div className='col s12'>
                                                <span>Enrolled On:</span>
                                                <span className='right'>{ moment(enrolment.enrolledOn).format('DD MMM YYYY hh:mm') }</span>
                                            </div>

                                            <div className='col s12' style={{ marginTop: '1em' }}>
                                                <button className='btn waves-effect waves-light'
                                                        onClick={ () => alert('Open classroom content page for student view.') }
                                                >
                                                    <i className="fas fa-door-open" />&nbsp;
                                                    Go to classroom
                                                </button>

                                                {
                                                    !enrolment.invoice.isPaid &&
                                                    <button className='btn waves-effect waves-light red right'
                                                            onClick={ () => props.invokeUnenrolFromClassroomRequest(props.authUser, enrolment.id) }
                                                    >
                                                        <i className="fas fa-trash-alt" />&nbsp;
                                                        Unenrol
                                                    </button>
                                                }
                                            </div>
                                        </div>

                                        <div id="marks" className='row' style={{ marginBottom: 0 }}>
                                            <div className='col s12'>
                                                <span>Overall Mark:</span>
                                                <span className='right'>{ enrolment.marksDetail.overallMarks } (provisional)</span>
                                            </div>
                                            <div className='clearfix' />

                                            <ul className="collapsible popout">
                                            {
                                                enrolment.marksDetail.markBreakdowns.map((mark, i) =>
                                                    <li key={ i }>
                                                        <div className="collapsible-header">
                                                            <i className="fas fa-tasks" /> { mark.taskName }
                                                        </div>
                                                        <div className="collapsible-body">
                                                            <div className='row' style={{ margin: 0 }}>
                                                                <div className='col s12'>
                                                                    <span>Marks (rewarded/total):</span>
                                                                    <span className='right'>{ mark.rewardedMarks }/{ mark.totalMarks }</span>
                                                                </div>

                                                                <div className='col s12'>
                                                                    <span>Marked On:</span>
                                                                    <span className='right'>{ moment(mark.markedOn).format('DD MMM YYYY hh:mm') }</span>
                                                                </div>

                                                                <div className='col s12'>
                                                                    <span>Comment:&nbsp;</span>
                                                                    <span className='right'>{ mark.comment || 'This is a very long long long comment to check for floating.' }</span>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </li>
                                                )
                                            }</ul>
                                        </div>

                                        <div id="invoice" className='row' style={{ marginBottom: 0 }}>
                                            {
                                                (
                                                    !enrolment.invoice.isPaid &&
                                                    <div className='row center-align'>You have an invoice of ${ enrolment.invoice.amount }. Please click the 'Pay' button and follow the instructions to pay.</div>
                                                ) ||
                                                <>
                                                    <div className='col s12'>
                                                        <span>Payment Method:</span>
                                                        <span className='right'>{ enrolment.invoice.paymentDetail?.paymentMethod || 'N/A' }</span>
                                                    </div>
                                                    <div className='col s12'>
                                                        <span>Payment Status:</span>
                                                        <span className='right'>{ enrolment.invoice.paymentDetail?.paymentStatus || 'N/A' }</span>
                                                    </div>
                                                    <div className='col s12'>
                                                        <span>Paid On:</span>
                                                        <span className='right'>{ moment(enrolment.invoice.paymentDetail?.paidOn || EMPTY_STRING).format('DD MMM YYYY hh:mm') }</span>
                                                    </div>
                                                    <div className='col s12'>
                                                        <span>Payment ID:</span>
                                                        <span className='right'>{ enrolment.invoice.paymentDetail?.paymentId || 'N/A' }</span>
                                                    </div>
                                                    <div className='col s12'>
                                                        <span>Charge ID:</span>
                                                        <span className='right'>{ enrolment.invoice.paymentDetail?.transactionId || 'N/A' }</span>
                                                    </div>
                                                    <div className='col s12'>
                                                        <span>Transaction ID:</span>
                                                        <span className='right'>{ enrolment.invoice.paymentDetail?.chargeId || 'N/A' }</span>
                                                    </div>
                                                </>
                                            }
                                        </div>
                                    </div>
                                </div>
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