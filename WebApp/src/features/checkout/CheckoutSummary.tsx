import React from 'react';
import {connect} from 'react-redux';
import _ from 'lodash';
import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import {EMPTY_STATUS, EMPTY_STRING, IStatusMessage} from "../../providers/helpers";
import {IEnrolment} from "../student/redux/interfaces";
import moment from "moment";
import { PayPalButton } from 'react-paypal-button-v2';
import GooglePayButton from '@google-pay/button-react';
import StripeCheckout from 'react-stripe-checkout';

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser
});

const mapActionsToProps = {};

const CheckoutSummary = (props: any) => {
    const [enrolment, setEnrolment] = React.useState(null as unknown as IEnrolment);
    const [paymentMethod, setPaymentMethod] = React.useState(EMPTY_STRING);
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);

    React.useEffect(() => {
        const storedEnrolment = localStorage.getItem('enrolment_checkoutSummary');
        //localStorage.removeItem('enrolment_checkoutSummary');

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
        const cardType = token.card.brand;
        const last4Digits = token.card.last4;
        const clientIp = token.client_ip;
        const tokenId = token.id;
        console.log(tokenId)
    }

    return (
        <div className='container' style={{ marginTop: '3em' }}>
            <div className='row'>
                <div className='col s12'>
                    <div className='card' style={{ marginTop: '2em' }}>
                        <div className='card-content'>
                            <p className='section-header'>
                                <i className="fas fa-shopping-cart" />&nbsp;
                                Checkout Summary
                            </p>

                            <div className='row' style={{ marginTop: '1em', marginBottom: 0 }}>
                                <div className='col m6 s12'>
                                    <p className='small-text'>Enrolled classroom</p>
                                    <div className='row'>
                                        <div className='col s9 push-s3 small-text'>
                                            <p>Class Name: <span className='right'>{ enrolment && enrolment.classroom.className }</span></p>
                                            <p>Teacher: <span className='right'>{ enrolment && enrolment.classroom.teacherName }</span></p>
                                            <p>Enrolled On: <span className='right'>{ enrolment && moment(enrolment.enrolledOn).format('DD MMM YYYY hh:mm') }</span></p>
                                        </div>
                                    </div>
                                </div>

                                <div className='col m6 s12'>
                                    <p className='small-text'>Invoice details</p>
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
                                                    const orderId = data.orderID;
                                                    const amount = authorization.purchase_units[0].payments.authorizations[0].amount.value;
                                                    const authorizationId = authorization.purchase_units[0].payments.authorizations[0].id;
                                                    //Capture payment at server
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
                                                    countryCode: 'AU',
                                                },
                                            }}
                                            onLoadPaymentData={ (paymentRequest: any) => {
                                                const paymentToken = JSON.parse(paymentRequest.paymentMethodData.tokenizationData.token);
                                                const cardType = paymentToken.card.brand;
                                                const last4Digits = paymentToken.card.last4;
                                                const clientIp = paymentToken.client_ip;
                                                const tokenId = paymentToken.id;
                                                //Capture payment at server
                                                console.log(tokenId)
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