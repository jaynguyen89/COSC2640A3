import React from 'react';
import {connect} from 'react-redux';
import {IAuthUser} from "../features/authentication/redux/interfaces";
import {clearAuthUser, invokeSignOutRequest} from "../features/authentication/redux/actions";
import {checkSession, EMPTY_STATUS, IActionResult, IStatusMessage, setGlobalMessage} from "../providers/helpers";
import * as authenticationConstants from "../features/authentication/redux/constants";
import Alert from "./Alert";
import Spinner from "./Spinner";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    unauthenticate: state.authenticationStore.unauthenticate
});

const mapActionsToProps = {
    invokeSignOutRequest,
    clearAuthUser
};

export interface IHeaderNav {
    authUser: IAuthUser,
    greetingName: string,
    location: string,
    invokeSignOutRequest: (auth: IAuthUser) => void,
    unauthenticate: IActionResult,
    clearAuthUser: () => void
}

const HeaderNav = (props: IHeaderNav) => {
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);

    React.useEffect(() => {
        if (props.unauthenticate.action === authenticationConstants.SIGNOUT_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.unauthenticate.error?.message);

        if (props.unauthenticate.action === authenticationConstants.SIGNOUT_REQUEST_SUCCESS) {
            setGlobalMessage({ messages: ['You have been signed out.'], type: 'success' } as IStatusMessage);
            props.clearAuthUser();
            window.location.href = '/';
        }
    }, [props.unauthenticate]);

    const goToSelectedPage = (location: string) => {
        if (location === 'home' && props.location.indexOf('home') === -1) window.location.href = '/home';
        // if (location === 'studentClassrooms') window.location.href = '/home';
        // if (location === 'studentEnrolments') window.location.href = '/home';
        // if (location === 'studentAllClassrooms') window.location.href = '/home';
        if (location === 'manage-classrooms' && props.location.indexOf('manage-classrooms') === -1) window.location.href = '/manage-classrooms';
        // if (location === 'teacherImportExport') window.location.href = '/home';
    }

    return (
        <div className='row'>
            <Alert { ...statusMessage } />
            { props.unauthenticate.action === authenticationConstants.SIGNOUT_REQUEST_SENT && <Spinner /> }

            <div className='col s7'>
                <h4 className='section-header'>
                    Hello { props.greetingName }!
                    <button className='btn waves-effect waves-light pink'
                            style={{ marginLeft: '1em' }}
                            onClick={ () => props.invokeSignOutRequest(props.authUser) }
                    >
                        <i className="fas fa-sign-out-alt" />
                        &nbsp; Sign out
                    </button>
                </h4>
            </div>

            <div className="input-field col s5">
                <select className="browser-default" style={{ marginTop: '0.5em' }}
                        value={ props.location }
                        onChange={ e => goToSelectedPage(e.target.value) }
                >
                    <option value="home" selected>My Profile & Account Details</option>
                    {
                        (
                            props.authUser.role === 0 &&
                            <>
                                <option value="studentClassrooms">My Classrooms & Marks</option>
                                <option value="studentEnrolments">My Enrolments, Invoices & Payments</option>
                                <option value="studentAllClassrooms">Browse for all classrooms</option>
                            </>
                        ) ||
                        <>
                            <option value="manage-classrooms">Manage my classrooms</option>
                            <option value="teacherImportExport">Import & Export My Data</option>
                        </>
                    }
                </select>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(HeaderNav);