import {IClassroomData} from "../../classroom/redux/interfaces";
import {IAuthUser} from "../../authentication/redux/interfaces";
import {IActionResult} from "../../../providers/helpers";

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
    isPaid: boolean | null
}

interface IMarkBreakdown {
    taskName: string,
    totalMarks: number,
    rewardedMarks: number,
    markedOn: string,
    comment: string
}