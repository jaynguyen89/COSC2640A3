import * as checkoutConstants from './constants';
import produce from 'immer';
import {DEFAULT_ACTION_RESULT, IActionResult} from "../../../providers/helpers";

interface ICheckoutStore {
    paypalCheckout: IActionResult,
    googleCheckout: IActionResult,
    cardCheckout: IActionResult
}

const initialState: ICheckoutStore = {
    paypalCheckout: DEFAULT_ACTION_RESULT,
    googleCheckout: DEFAULT_ACTION_RESULT,
    cardCheckout: DEFAULT_ACTION_RESULT
}

const reducer = produce((state, action) => {
    switch (action.type) {
        case checkoutConstants.PAYPAL_CHECKOUT_REQUEST_SENT:
            state.paypalCheckout.action = checkoutConstants.PAYPAL_CHECKOUT_REQUEST_SENT;
            state.paypalCheckout.payload = null;
            state.paypalCheckout.error = null;
            return;
        case checkoutConstants.PAYPAL_CHECKOUT_REQUEST_SUCCESS:
            state.paypalCheckout.action = checkoutConstants.PAYPAL_CHECKOUT_REQUEST_SUCCESS;
            state.paypalCheckout.payload = action.payload;
            state.paypalCheckout.error = null;
            return;
        case checkoutConstants.PAYPAL_CHECKOUT_REQUEST_FAILED:
            state.paypalCheckout.action = checkoutConstants.PAYPAL_CHECKOUT_REQUEST_FAILED;
            state.paypalCheckout.payload = null;
            state.paypalCheckout.error = action.error;
            return;
        case checkoutConstants.GOOGLE_CHECKOUT_REQUEST_SENT:
            state.googleCheckout.action = checkoutConstants.GOOGLE_CHECKOUT_REQUEST_SENT;
            state.googleCheckout.payload = null;
            state.googleCheckout.error = null;
            return;
        case checkoutConstants.GOOGLE_CHECKOUT_REQUEST_SUCCESS:
            state.googleCheckout.action = checkoutConstants.GOOGLE_CHECKOUT_REQUEST_SUCCESS;
            state.googleCheckout.payload = action.payload;
            state.googleCheckout.error = null;
            return;
        case checkoutConstants.GOOGLE_CHECKOUT_REQUEST_FAILED:
            state.googleCheckout.action = checkoutConstants.GOOGLE_CHECKOUT_REQUEST_FAILED;
            state.googleCheckout.payload = null;
            state.googleCheckout.error = action.error;
            return;
        case checkoutConstants.CARD_CHECKOUT_REQUEST_SENT:
            state.cardCheckout.action = checkoutConstants.CARD_CHECKOUT_REQUEST_SENT;
            state.cardCheckout.payload = null;
            state.cardCheckout.error = null;
            return;
        case checkoutConstants.CARD_CHECKOUT_REQUEST_SUCCESS:
            state.cardCheckout.action = checkoutConstants.CARD_CHECKOUT_REQUEST_SUCCESS;
            state.cardCheckout.payload = action.payload;
            state.cardCheckout.error = null;
            return;
        case checkoutConstants.CARD_CHECKOUT_REQUEST_FAILED:
            state.cardCheckout.action = checkoutConstants.CARD_CHECKOUT_REQUEST_FAILED;
            state.cardCheckout.payload = null;
            state.cardCheckout.error = action.error;
            return;
        default:
            return;
    }
}, initialState);

export default reducer;