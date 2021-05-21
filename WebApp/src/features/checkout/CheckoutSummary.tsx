import React from 'react';
import {connect} from 'react-redux';
import _ from 'lodash';
import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import {
    checkSession,
    EMPTY_STATUS,
    EMPTY_STRING,
    IResponse,
    IStatusMessage
} from "../../providers/helpers";
import {IEnrolment} from "../student/redux/interfaces";
import moment from "moment";
import { PayPalButton } from 'react-paypal-button-v2';
import GooglePayButton from '@google-pay/button-react';
import StripeCheckout from 'react-stripe-checkout';
import {invokeCardCheckoutRequest, invokeGoogleCheckoutRequest, invokePaypalCheckoutRequest} from "./redux/actions";
import {ICheckoutSummary, IPayment, IPaypalAuthorization, IStripeAuthorization} from "./redux/interfaces";
import * as checkoutConstants from './redux/constants';
import {clearAuthUser} from "../authentication/redux/actions";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    paypalCheckout: state.checkoutStore.paypalCheckout,
    googleCheckout: state.checkoutStore.googleCheckout,
    cardCheckout: state.checkoutStore.cardCheckout,
});

const mapActionsToProps = {
    invokePaypalCheckoutRequest,
    invokeGoogleCheckoutRequest,
    invokeCardCheckoutRequest,
    clearAuthUser
};

const CheckoutSummary = (props: ICheckoutSummary) => {
    const [enrolment, setEnrolment] = React.useState(null as unknown as IEnrolment);
    const [paymentMethod, setPaymentMethod] = React.useState(EMPTY_STRING);
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [payment, setPayment] = React.useState(null as unknown as IPayment);

    React.useEffect(() => {
        const storedEnrolment = localStorage.getItem('enrolment_checkoutSummary');

        if (storedEnrolment) setEnrolment(JSON.parse(storedEnrolment) as IEnrolment);
        else setStatusMessage({
            messages: ['Unable to get enrolment and invoice data. Please reload page to try again.'],
            type: 'error',
            closeAlert: () => setStatusMessage(EMPTY_STATUS)
        } as IStatusMessage);
    }, []);

    const updatePaymentMethod = (method: string) => {
        if (method === paymentMethod) {
            setPaymentMethod(EMPTY_STRING);
            return;
        }

        setPaymentMethod(method);
    }

    const processStripeCheckout = (token: any) => {
        const paymentAuthorization: IStripeAuthorization = {
            cardType: token.card.brand,
            last4Digits: token.card.last4,
            tokenId: token.id,
            details: {
                classroomId: enrolment.classroom.id,
                className: enrolment.classroom.className,
                amount: enrolment.invoice.amount
            }
        };

        setStatusMessage(EMPTY_STATUS);
        if (paymentMethod === 'google') props.invokeGoogleCheckoutRequest(props.authUser, paymentAuthorization, enrolment.id);
        else props.invokeCardCheckoutRequest(props.authUser, paymentAuthorization, enrolment.id);

        setPaymentMethod(EMPTY_STRING);
    }

    React.useEffect(() => {
        if (props.paypalCheckout.action === checkoutConstants.PAYPAL_CHECKOUT_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.paypalCheckout.error?.message);

        if (props.paypalCheckout.action === checkoutConstants.PAYPAL_CHECKOUT_REQUEST_SUCCESS)
            checkResults(props.paypalCheckout.payload);
    }, [props.paypalCheckout]);

    React.useEffect(() => {
        if (props.googleCheckout.action === checkoutConstants.GOOGLE_CHECKOUT_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.googleCheckout.error?.message);

        if (props.googleCheckout.action === checkoutConstants.GOOGLE_CHECKOUT_REQUEST_SUCCESS)
            checkResults(props.googleCheckout.payload);
    }, [props.googleCheckout]);

    React.useEffect(() => {
        if (props.cardCheckout.action === checkoutConstants.CARD_CHECKOUT_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.cardCheckout.error?.message);

        if (props.cardCheckout.action === checkoutConstants.CARD_CHECKOUT_REQUEST_SUCCESS)
            checkResults(props.cardCheckout.payload);
    }, [props.cardCheckout]);

    const checkResults = (result: IResponse | null) => {
        if (result === null) setStatusMessage({ messages: ['Failed to send request to server. Payment was not processed. Please try again.'], type: 'error' } as IStatusMessage);
        else if (result.result === 0) setStatusMessage({ messages: result.messages, type: 'error' } as IStatusMessage);
        else {
            setStatusMessage({ messages: ['We have got your payment. Your invoice has been updated. Thank you!'], type: 'error' } as IStatusMessage);
            setPayment(result.data as IPayment);
        }
    }

    return (
        <div className='container' style={{ marginTop: '3em' }}>
            <div className='row'>
                <div className='col s12'>
                    <div className='card' style={{ marginTop: '2em' }}>
                        <div className='card-content'>
                            <Alert { ...statusMessage } closeAlert={ () => setStatusMessage(EMPTY_STATUS) } />
                            {
                                payment &&
                                <div className='alert alert-success small-text'>
                                    <div className='row' style={{ margin: 0 }}>
                                        <div className='col s6'>
                                            <p>Payment ID: <span className='right'>{ payment.paymentId }</span></p>
                                            <p>Payment Method: <span className='right'>{ payment.paymentMethod }</span></p>
                                            <p>Payment Status: <span className='right'>{ payment.paymentStatus }</span></p>
                                            <p>Transaction ID: <span className='right'>{ payment.transactionId || 'N/A' }</span></p>
                                        </div>
                                        <div className='col s6'>
                                            <p>Charge ID: <span className='right'>{ payment.chargeId || 'N/A' }</span></p>
                                            <p>Paid Amount: <span className='right'>${ payment.dueAmount }</span></p>
                                            <p>Money Captured: <span className='right'>{ (payment.isPaid && 'YES') || 'N/A' }</span></p>
                                            <p>Paid On: <span className='right'>{ moment(payment.paidOn).format('DD MMM YYYY hh:mm') }</span></p>
                                        </div>
                                    </div>
                                </div>
                            }

                            <p className='section-header'>
                                <i className="fas fa-shopping-cart" />&nbsp;
                                Checkout Summary
                            </p>

                            <div className='row' style={{ marginTop: '1em', marginBottom: 0 }}>
                                <div className='col m6 s12'>
                                    <p className='small-text'>Enrolled classroom:</p>
                                    <div className='row'>
                                        <div className='col s9 push-s3 small-text'>
                                            <p>Class Name: <span className='right'>{ enrolment && enrolment.classroom.className }</span></p>
                                            <p>Teacher: <span className='right'>{ enrolment && enrolment.classroom.teacherName }</span></p>
                                            <p>Enrolled On: <span className='right'>{ enrolment && moment(enrolment.enrolledOn).format('DD MMM YYYY hh:mm') }</span></p>
                                        </div>
                                    </div>
                                </div>

                                <div className='col m6 s12'>
                                    <p className='small-text'>Invoice details:</p>
                                    <div className='row'>
                                        <div className='col s9 push-s3 small-text'>
                                            <p>Base Price: <span className='right'>${ _.round(enrolment && enrolment.invoice.amount / 1.135, 2) }</span></p>
                                            <p>GST: <span className='right'>${ _.round(enrolment && enrolment.invoice.amount * 0.1 / 1.135, 2) }</span></p>
                                            <p>Surcharge 3.5%: <span className='right'>${ _.round(enrolment && enrolment.invoice.amount  * 0.035 / 1.135, 2) }</span></p>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <p>Total payable: <b>${ enrolment && enrolment.invoice.amount }</b></p>
                            <p>Please select your payment method:</p>
                            <div className='row'>
                                <div className='col s12 center-align'>
                                    <img src='/assets/paypal.png' width='75px'
                                         className={ (paymentMethod === 'paypal' && 'responsive-img payment-logo payment-logo-active') || 'responsive-img payment-logo' }
                                         onClick={ () => updatePaymentMethod('paypal') }
                                    />

                                    <img src='/assets/google-pay.png' width='75px'
                                         className={ (paymentMethod === 'google' && 'responsive-img payment-logo payment-logo-active') || 'responsive-img payment-logo' }
                                         onClick={ () => updatePaymentMethod('google') }
                                    />

                                    <img src='/assets/visa-card.png' width='75px'
                                         className={ (paymentMethod === 'card' && 'responsive-img payment-logo payment-logo-active') || 'responsive-img payment-logo' }
                                         onClick={ () => updatePaymentMethod('card') }
                                    />
                                </div>
                                {
                                    (
                                        props.paypalCheckout.action === checkoutConstants.PAYPAL_CHECKOUT_REQUEST_SENT ||
                                        props.googleCheckout.action === checkoutConstants.GOOGLE_CHECKOUT_REQUEST_SENT ||
                                        props.cardCheckout.action === checkoutConstants.CARD_CHECKOUT_REQUEST_SENT
                                    ) && <Spinner />
                                }

                                {
                                    paymentMethod === 'paypal' &&
                                    <div className='col s12 center-align'>
                                        <PayPalButton
                                            options={{
                                                clientId: 'ASRvVBDpjSkg9hMfMD1h_bEAcVzMIg91FXtqhA6pRHrSluyPwuT7-rpSgoPfleFh757E0XcZ6tLCZYtG',
                                                currency: 'AUD',
                                                intent: 'AUTHORIZE'
                                            }}
                                            onError={ () => setStatusMessage({
                                                messages: ['An issue happened while processing your payment. No money has been paid. Please try again.'],
                                                type: 'success',
                                                closeAlert: () => setStatusMessage(EMPTY_STATUS) } as IStatusMessage
                                            )}
                                            createOrder={ (data: any, actions: any) => {
                                                return actions.order.create({
                                                    purchase_units: [{
                                                        amount: {
                                                            currency_code: 'AUD',
                                                            value: enrolment.invoice.amount
                                                        }
                                                    }]
                                                });
                                            }}
                                            onApprove={ (data: any, actions: any) => {
                                                actions.order.authorize().then((authorization: any) => {
                                                    if (authorization.status !== 'COMPLETED') {
                                                        setStatusMessage({ messages: ['Payment has been cancelled.'], type: 'error', closeAlert: () => setStatusMessage(EMPTY_STATUS) } as IStatusMessage);
                                                        return;
                                                    }

                                                    setStatusMessage(EMPTY_STATUS);
                                                    const paypalAuthorization: IPaypalAuthorization = {
                                                        paypalEmail: authorization.payer.email_address,
                                                        authorizationId: authorization.purchase_units[0].payments.authorizations[0].id,
                                                        amount: authorization.purchase_units[0].payments.authorizations[0].amount.value,
                                                        orderId: data.orderID
                                                    };

                                                    props.invokePaypalCheckoutRequest(props.authUser, paypalAuthorization, enrolment.id);
                                                });
                                            }}
                                        />
                                    </div>
                                }

                                {
                                    paymentMethod === 'google' &&
                                    <div className='col s12 center-align'>
                                        <GooglePayButton
                                            environment="TEST"
                                            paymentRequest={{
                                                apiVersion: 2,
                                                apiVersionMinor: 0,
                                                allowedPaymentMethods: [
                                                    {
                                                        type: 'CARD',
                                                        parameters: {
                                                            allowedAuthMethods: ['PAN_ONLY', 'CRYPTOGRAM_3DS'],
                                                            allowedCardNetworks: ['MASTERCARD', 'VISA'],
                                                        },
                                                        tokenizationSpecification: {
                                                            type: 'PAYMENT_GATEWAY',
                                                            parameters: {
                                                                gateway: 'stripe',
                                                                'stripe:version': '2020-08-27',
                                                                'stripe:publishableKey': "pk_test_51HQDZND2FG7NncIEj68F5ie7Yc6VKR7y5r0aMkoaf3OD5CUIcqHBCYq3Wb2biu3D1jie5wjUKdsfwh3kdWG6flgJ00KdGXIjMp"
                                                            },
                                                        },
                                                    },
                                                ],
                                                merchantInfo: {
                                                    merchantId: 'acct_1HQDZND2FG7NncIE',
                                                    merchantName: 'COSC2640A3',
                                                },
                                                transactionInfo: {
                                                    totalPriceStatus: 'FINAL',
                                                    totalPriceLabel: 'Total Payable',
                                                    totalPrice: enrolment.invoice.amount.toString(),
                                                    currencyCode: 'AUD',
                                                    countryCode: 'AU'
                                                }
                                            }}
                                            onLoadPaymentData={ (paymentRequest: any) => {
                                                const paymentToken = JSON.parse(paymentRequest.paymentMethodData.tokenizationData.token);
                                                processStripeCheckout(paymentToken);
                                            }}
                                        />
                                    </div>
                                }

                                {
                                    paymentMethod === 'card' &&
                                    <div className='col s12 center-align'>
                                        <StripeCheckout
                                            amount={ enrolment.invoice.amount }
                                            description="COSC2640A3"
                                            image="https://logos-world.net/wp-content/uploads/2021/03/Stripe-Emblem.png"
                                            name="cosc2640a3.com"
                                            stripeKey="pk_test_51HQDZND2FG7NncIEj68F5ie7Yc6VKR7y5r0aMkoaf3OD5CUIcqHBCYq3Wb2biu3D1jie5wjUKdsfwh3kdWG6flgJ00KdGXIjMp"
                                            token={ processStripeCheckout }
                                            panelLabel='Pay {{ amount }}'
                                        >
                                            <button className='btn waves-effect waves-light'>
                                                <i className="fas fa-coins"/>&nbsp;
                                                Checkout by Bank Card with Stripe
                                            </button>
                                        </StripeCheckout>
                                    </div>
                                }
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(CheckoutSummary);