import React from 'react';
import moment from "moment";
import {EMPTY_STRING} from "../../providers/helpers";
import {IEnrolmentInfo} from "./redux/interfaces";

const EnrolmentInfo = (props: IEnrolmentInfo) => {
    return (
        <div className='card'>
            <div className='card-content'>
                {

                    props.viewByStudent
                    ?
                        (
                            props.enrolment.invoice.isPaid &&
                            <>
                                <i className="fas fa-check-circle green-text" />&nbsp;Invoice Paid. Thank you!
                            </>
                        ) ||
                        <>
                            <i className="fas fa-info-circle red-text" />&nbsp;Invoice: ${ props.enrolment.invoice.amount }
                            <button className='btn waves-effect waves-light right'
                                    onClick={ () => {
                                        localStorage.setItem('enrolment_checkoutSummary', JSON.stringify(props.enrolment));
                                        window.location.href = '/checkout-summary';
                                    }}
                            >
                                <i className="fas fa-shopping-cart" />&nbsp;Pay
                            </button>
                        </>
                    :
                        <>
                            <button className='btn waves-effect waves-light'
                                    onClick={ props.handleUpdateMarksBtn }
                            >
                                Update Marks
                            </button>

                            {
                                !props.enrolment.invoice.isPaid &&
                                <button className='btn waves-effect waves-light red right'
                                        onClick={ props.handleUnenrolBtn }
                                >
                                    Cancel Enrolment
                                </button>
                            }
                        </>
                }
            </div>

            <div className="card-tabs">
                <ul className="tabs tabs-fixed-width teal lighten-5">
                    {
                        (
                            props.viewByStudent &&
                            <li className="tab"><a href="#classroom">Classroom</a></li>
                        ) || <li className="tab"><a href="#student">Student</a></li>
                    }
                    <li className="tab"><a href="#marks">Marks</a></li>
                    <li className="tab"><a href="#invoice">Invoice</a></li>
                </ul>
            </div>

            <div className="card-content grey lighten-4 small-text">
                {
                    (
                        props.viewByStudent &&
                        <div id="classroom" className='row' style={{ marginBottom: 0 }}>
                            <div className='col s12'>
                                <span>Class Name:</span>
                                <span className='right'>{ props.enrolment.classroom.className }</span>
                            </div>

                            <div className='col s12'>
                                <span>Teacher:</span>
                                <span className='right'>{ props.enrolment.classroom.teacherName }</span>
                            </div>

                            <div className='col s12'>
                                <span>Enrolled On:</span>
                                <span className='right'>{ moment(props.enrolment.enrolledOn).format('DD MMM YYYY hh:mm') }</span>
                            </div>

                            <div className='col s12' style={{ marginTop: '1em' }}>
                                <button className='btn waves-effect waves-light'
                                        onClick={() => alert('Open classroom content page for student view.')}
                                >
                                    <i className="fas fa-door-open"/>&nbsp;
                                    Go to classroom
                                </button>

                                {
                                    !props.enrolment.invoice.isPaid &&
                                    <button className='btn waves-effect waves-light red right'
                                            onClick={ props.handleUnenrolBtn }
                                    >
                                        <i className="fas fa-trash-alt"/>&nbsp;
                                        Unenrol
                                    </button>
                                }
                            </div>
                        </div>
                    ) ||
                    <div id="student" className='row' style={{ marginBottom: 0 }}>
                        <div className='col s12'>
                            <span>Student Name:</span>
                            <span className='right'>{ props.enrolment.student.preferredName }</span>
                        </div>

                        <div className='col s12'>
                            <span>Username:</span>
                            <span className='right'>{ props.enrolment.student.username }</span>
                        </div>

                        <div className='col s12'>
                            <span>School:</span>
                            <span className='right'>{ props.enrolment.student.schoolName }</span>
                        </div>

                        <div className='col s12'>
                            <span>Faculty:</span>
                            <span className='right'>{ props.enrolment.student.faculty }</span>
                        </div>

                        <div className='col s12'>
                            <span>Email:</span>
                            <span className='right'>{ props.enrolment.student.email }</span>
                        </div>

                        <div className='col s12'>
                            <span>Contact:</span>
                            <span className='right'>{ props.enrolment.student.phoneNumber }</span>
                        </div>

                        <div className='col s12'>
                            <span>Website:</span>
                            <span className='right'>{ props.enrolment.student.personalUrl }</span>
                        </div>
                    </div>
                }

                <div id="marks" className='row' style={{ marginBottom: 0 }}>
                    <div className='col s12 center-align'>
                        <span>Overall Mark:&nbsp;</span>
                        <span>{ props.enrolment.marksDetail.overallMarks }</span>
                    </div>
                    <div className='clearfix' />

                    {
                        (props.enrolment.marksDetail.markBreakdowns.length === 0 && <p>No marks have been released.</p>) ||
                        <ul className="collapsible popout">
                            {
                                props.enrolment.marksDetail.markBreakdowns.map((mark, i) =>
                                    <li key={i}>
                                        <div className="collapsible-header">
                                            <i className="fas fa-tasks"/> { mark.taskName }
                                        </div>
                                        <div className="collapsible-body">
                                            <div className='row' style={{ margin: 0 }}>
                                                <div className='col s12'>
                                                    <span>Marks (rewarded/total):</span>
                                                    <span
                                                        className='right'>{ mark.rewardedMarks }/{ mark.totalMarks }</span>
                                                </div>

                                                <div className='col s12'>
                                                    <span>Marked On:</span>
                                                    <span
                                                        className='right'>{ moment(mark.markedOn).format('DD MMM YYYY hh:mm') }</span>
                                                </div>

                                                <div className='col s12'>
                                                    <span>Comment:&nbsp;</span>
                                                    <span className='right'>{ mark.comment || 'N/A' }</span>
                                                </div>
                                            </div>
                                        </div>
                                    </li>
                                )
                            }
                        </ul>
                    }
                </div>

                <div id="invoice" className='row' style={{ marginBottom: 0 }}>
                    {
                        (
                            !props.enrolment.invoice.isPaid &&
                            <div className='row center-align'>
                                {
                                    (
                                        props.viewByStudent && `You have an invoice of $${ props.enrolment.invoice.amount }. Please click the 'Pay' button and follow the instructions to pay.`
                                    ) || 'Student hasn\'t paid for this enrolment.'
                                }
                            </div>
                        ) ||
                        <>
                            <div className='col s12'>
                                <span>Payment Method:</span>
                                <span className='right'>{ props.enrolment.invoice.paymentDetail?.paymentMethod || 'N/A' }</span>
                            </div>
                            <div className='col s12'>
                                <span>Payment Status:</span>
                                <span className='right'>{ props.enrolment.invoice.paymentDetail?.paymentStatus || 'N/A' }</span>
                            </div>
                            <div className='col s12'>
                                <span>Paid On:</span>
                                <span className='right'>{ moment(props.enrolment.invoice.paymentDetail?.paidOn || EMPTY_STRING).format('DD MMM YYYY hh:mm') }</span>
                            </div>
                            <div className='col s12'>
                                <span>Payment ID:</span>
                                <span className='right'>{ props.enrolment.invoice.paymentDetail?.paymentId || 'N/A' }</span>
                            </div>
                            <div className='col s12'>
                                <span>Charge ID:</span>
                                <span className='right'>{ props.enrolment.invoice.paymentDetail?.transactionId || 'N/A' }</span>
                            </div>
                            <div className='col s12'>
                                <span>Transaction ID:</span>
                                <span className='right'>{ props.enrolment.invoice.paymentDetail?.chargeId || 'N/A' }</span>
                            </div>
                        </>
                    }
                </div>
            </div>
        </div>
    );
}

export default EnrolmentInfo;