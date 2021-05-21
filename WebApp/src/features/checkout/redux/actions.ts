import * as checkoutConstants from './constants';
import * as checkoutServices from './services';
import {IAuthUser} from "../../authentication/redux/interfaces";
import {IPaypalAuthorization, IStripeAuthorization} from "./interfaces";

export const invokePaypalCheckoutRequest = (auth: IAuthUser, paypalAuth: IPaypalAuthorization, enrolmentId: string) => {
    return (dispatch: any) => {
        dispatch({ type: checkoutConstants.PAYPAL_CHECKOUT_REQUEST_SENT });

        checkoutServices.sendPaypalCheckoutRequest(auth, paypalAuth, enrolmentId)
            .then(response => dispatch({
                type: checkoutConstants.PAYPAL_CHECKOUT_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: checkoutConstants.PAYPAL_CHECKOUT_REQUEST_FAILED,
                error
            }));
    };
}

export const invokeGoogleCheckoutRequest = (auth: IAuthUser, googleAuth: IStripeAuthorization, enrolmentId: string) => {
    return (dispatch: any) => {
        dispatch({ type: checkoutConstants.GOOGLE_CHECKOUT_REQUEST_SENT });

        checkoutServices.sendGoogleCheckoutRequest(auth, googleAuth, enrolmentId)
            .then(response => dispatch({
                type: checkoutConstants.GOOGLE_CHECKOUT_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: checkoutConstants.GOOGLE_CHECKOUT_REQUEST_FAILED,
                error
            }));
    };
}

export const invokeCardCheckoutRequest = (auth: IAuthUser, cardAuth: IStripeAuthorization, enrolmentId: string) => {
    return (dispatch: any) => {
        dispatch({ type: checkoutConstants.CARD_CHECKOUT_REQUEST_SENT });

        checkoutServices.sendCardCheckoutRequest(auth, cardAuth, enrolmentId)
            .then(response => dispatch({
                type: checkoutConstants.CARD_CHECKOUT_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: checkoutConstants.CARD_CHECKOUT_REQUEST_FAILED,
                error
            }));
    };
}