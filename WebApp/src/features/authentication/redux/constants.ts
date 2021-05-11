// When app launches
export const LOAD_AUTH_DATA = 'LOAD_AUTH_DATA';
export type T_LOAD_AUTH_DATA = typeof LOAD_AUTH_DATA;

// When sign-in success
export const AUTHENTICATED = 'AUTHENTICATED';
export type T_AUTHENTICATED = typeof AUTHENTICATED;

// When sign-out success
export const NO_AUTHENTICATION = 'NO_AUTHENTICATION';
export type T_NO_AUTHENTICATION = typeof NO_AUTHENTICATION;

// Constants for handling logging-in
export const AUTHENTICATION_BEGIN = 'AUTHENTICATION_BEGIN';
export type T_AUTHENTICATION_BEGIN = typeof AUTHENTICATION_BEGIN;

export const AUTHENTICATION_SUCCESS = 'AUTHENTICATION_SUCCESS';
export type T_AUTHENTICATION_SUCCESS = typeof AUTHENTICATION_SUCCESS;

export const AUTHENTICATION_FAILED = 'AUTHENTICATION_FAILED';
export type T_AUTHENTICATION_FAILED = typeof AUTHENTICATION_FAILED;

// Constants for handling registration
export const REGISTRATION_REQUEST_SENT = 'REGISTRATION_REQUEST_SENT';
export type T_REGISTRATION_REQUEST_SENT = typeof REGISTRATION_REQUEST_SENT;

export const REGISTRATION_REQUEST_SUCCESS = 'REGISTRATION_REQUEST_SUCCESS';
export type T_REGISTRATION_REQUEST_SUCCESS = typeof REGISTRATION_REQUEST_SUCCESS;

export const REGISTRATION_REQUEST_FAILED = 'REGISTRATION_REQUEST_FAILED';
export type T_REGISTRATION_REQUEST_FAILED = typeof REGISTRATION_REQUEST_FAILED;

// Constants for handling sign-out
export const SIGNOUT_REQUEST_SENT = 'SIGNOUT_REQUEST_SENT';
export type T_SIGNOUT_REQUEST_SENT = typeof SIGNOUT_REQUEST_SENT;

export const SIGNOUT_REQUEST_SUCCESS = 'SIGNOUT_REQUEST_SUCCESS';
export type T_SIGNOUT_REQUEST_SUCCESS = typeof SIGNOUT_REQUEST_SUCCESS;

export const SIGNOUT_REQUEST_FAILED = 'SIGNOUT_REQUEST_FAILED';
export type T_SIGNOUT_REQUEST_FAILED = typeof SIGNOUT_REQUEST_FAILED;

// Constants for confirming TFA PIN
export const CONFIRM_TFA_REQUEST_SENT = 'CONFIRM_TFA_REQUEST_SENT';
export type T_CONFIRM_TFA_REQUEST_SENT = typeof CONFIRM_TFA_REQUEST_SENT;

export const CONFIRM_TFA_REQUEST_SUCCESS = 'CONFIRM_TFA_REQUEST_SUCCESS';
export type T_CONFIRM_TFA_REQUEST_SUCCESS = typeof CONFIRM_TFA_REQUEST_SUCCESS;

export const CONFIRM_TFA_REQUEST_FAILED = 'CONFIRM_TFA_REQUEST_FAILED';
export type T_CONFIRM_TFA_REQUEST_FAILED = typeof CONFIRM_TFA_REQUEST_FAILED;

// Constants for account activation
export const ACCOUNT_ACTIVATION_REQUEST_SENT = 'ACCOUNT_ACTIVATION_REQUEST_SENT';
export type T_ACCOUNT_ACTIVATION_REQUEST_SENT = typeof ACCOUNT_ACTIVATION_REQUEST_SENT;

export const ACCOUNT_ACTIVATION_REQUEST_SUCCESS = 'ACCOUNT_ACTIVATION_REQUEST_SUCCESS';
export type T_ACCOUNT_ACTIVATION_REQUEST_SUCCESS = typeof ACCOUNT_ACTIVATION_REQUEST_SUCCESS;

export const ACCOUNT_ACTIVATION_REQUEST_FAILED = 'ACCOUNT_ACTIVATION_REQUEST_FAILED';
export type T_ACCOUNT_ACTIVATION_REQUEST_FAILED = typeof ACCOUNT_ACTIVATION_REQUEST_FAILED;

// Constant for sending TFA PIN to SMS and Email
export const SEND_PIN_TO_SMS_EMAIL_REQUEST_SENT = 'SEND_PIN_TO_SMS_EMAIL_REQUEST_SENT';
export type T_SEND_PIN_TO_SMS_EMAIL_REQUEST_SENT = typeof SEND_PIN_TO_SMS_EMAIL_REQUEST_SENT;

export const SEND_PIN_TO_SMS_EMAIL_REQUEST_SUCCESS = 'SEND_PIN_TO_SMS_EMAIL_REQUEST_SUCCESS';
export type T_SEND_PIN_TO_SMS_EMAIL_REQUEST_SUCCESS = typeof SEND_PIN_TO_SMS_EMAIL_REQUEST_SUCCESS;

export const SEND_PIN_TO_SMS_EMAIL_REQUEST_FAILED = 'SEND_PIN_TO_SMS_EMAIL_REQUEST_FAILED';
export type T_SEND_PIN_TO_SMS_EMAIL_REQUEST_FAILED = typeof SEND_PIN_TO_SMS_EMAIL_REQUEST_FAILED;

// Constants for recovering password (Forgot Password)
export const FORGOT_PASSWORD_REQUEST_SENT = 'FORGOT_PASSWORD_REQUEST_SENT';
export type T_FORGOT_PASSWORD_REQUEST_SENT = typeof FORGOT_PASSWORD_REQUEST_SENT;

export const FORGOT_PASSWORD_REQUEST_SUCCESS = 'FORGOT_PASSWORD_REQUEST_SUCCESS';
export type T_FORGOT_PASSWORD_REQUEST_SUCCESS = typeof FORGOT_PASSWORD_REQUEST_SUCCESS;

export const FORGOT_PASSWORD_REQUEST_FAILED = 'FORGOT_PASSWORD_REQUEST_FAILED';
export type T_FORGOT_PASSWORD_REQUEST_FAILED = typeof FORGOT_PASSWORD_REQUEST_FAILED;