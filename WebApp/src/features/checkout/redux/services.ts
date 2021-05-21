import {IAuthUser} from "../../authentication/redux/interfaces";
import {IPaypalAuthorization, IStripeAuthorization} from "./interfaces";
import {IIssue, IResponse} from "../../../providers/helpers";
import {sendRequestForResult} from "../../../providers/serviceProvider";

const PAYMENT_ENDPOINT = 'payment/';

export const sendPaypalCheckoutRequest = (
    auth: IAuthUser, paypalAuth: IPaypalAuthorization, enrolmentId: string
): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ PAYMENT_ENDPOINT }paypal/${ enrolmentId }`,
        auth,
        paypalAuth,
        null
    );
}

export const sendGoogleCheckoutRequest = (
    auth: IAuthUser, googleAuth: IStripeAuthorization, enrolmentId: string
): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ PAYMENT_ENDPOINT }paypal/${ enrolmentId }`,
        auth,
        googleAuth,
        null
    );
}

export const sendCardCheckoutRequest = (
    auth: IAuthUser, cardAuth: IStripeAuthorization, enrolmentId: string
): Promise<IResponse | IIssue> => {
    return sendRequestForResult(
        `${ PAYMENT_ENDPOINT }paypal/${ enrolmentId }`,
        auth,
        cardAuth,
        null
    );
}