import {IClassroomData} from "../../classroom/redux/interfaces";
import {IAuthUser} from "../../authentication/redux/interfaces";
import {EMPTY_STRING, IActionResult} from "../../../providers/helpers";
import {IStudentDetail} from "../../homepage/redux/interfaces";

export interface IEnrolmentList {
    authUser: IAuthUser,
    clearAuthUser: () => void,
    invokeGetStudentEnrolmentsRequest: (auth: IAuthUser) => void,
    getStudentEnrolments: IActionResult,
    invokeUnenrolFromClassroomRequest: (auth: IAuthUser, enrolmentId: string) => void,
    unenrolClassroom: IActionResult
}

export interface IEnrolment {
    id: string,
    student: IStudentDetail,
    classroom: IClassroomData,
    invoice: IInvoice,
    marksDetail: IMarkDetail
    enrolledOn: string
}

interface IInvoice {
    id: string,
    amount: number,
    isPaid: boolean
    paymentDetail: IPaymentDetail | null
}

interface IPaymentDetail {
    paymentMethod: string,
    paymentId: string,
    transactionId: string,
    chargeId: string,
    paymentStatus: string,
    paidOn: string
}

interface IMarkDetail {
    overallMarks: number | null,
    markBreakdowns: Array<IMarkBreakdown>
    isPassed: boolean | null
}

export interface IMarkBreakdown {
    taskName: string,
    totalMarks: number,
    rewardedMarks: number,
    markedOn: string,
    comment: string
}

export const defaultMarkBreakdown : IMarkBreakdown = {
    taskName: EMPTY_STRING,
    totalMarks: 100,
    rewardedMarks: 100,
    markedOn: EMPTY_STRING,
    comment: EMPTY_STRING
}

export interface IEnrolmentInfo {
    enrolment: IEnrolment,
    viewByStudent?: true,
    handleUnenrolBtn: any
    handleUpdateMarksBtn?: any
}