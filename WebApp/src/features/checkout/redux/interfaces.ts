import {IActionResult} from "../../../providers/helpers";
import {IAuthUser} from "../../authentication/redux/interfaces";

export interface ICheckoutSummary {
    authUser: IAuthUser,
    clearAuthUser: () => void,
    paypalCheckout: IActionResult,
    googleCheckout: IActionResult,
    cardCheckout: IActionResult,
    invokePaypalCheckoutRequest: (auth: IAuthUser, paypalAuth: IPaypalAuthorization, enrolmentId: string) => void,
    invokeGoogleCheckoutRequest: (auth: IAuthUser, googleAuth: IStripeAuthorization, enrolmentId: string) => void,
    invokeCardCheckoutRequest: (auth: IAuthUser, cardAuth: IStripeAuthorization, enrolmentId: string) => void
}

export interface IPaypalAuthorization {
    paypalEmail: string,
    orderId: string,
    amount: number,
    authorizationId: string
}

export interface IStripeAuthorization {
    cardType: string,
    last4Digits: string,
    tokenId: string,
    details: {
        classroomId: string,
        className: string,
        amount: number
    }
}

export interface IPayment {
    id: string,
    paymentId: string,
    paymentMethod: string,
    paymentStatus: string,
    transactionId: string,
    chargeId: string,
    dueAmount: number,
    isPaid: boolean,
    paidOn: string,
}