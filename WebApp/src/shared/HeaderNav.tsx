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
        const currentLocation = window.location.href.split('/').pop();

        if (location === 'home' && currentLocation !== 'home') window.location.href = '/home';
        if (location === 'browse-classrooms' && currentLocation !== 'browse-classrooms') window.location.href = '/browse-classrooms';
        if (location === 'manage-classrooms' && currentLocation !== 'manage-classrooms') window.location.href = '/manage-classrooms';
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
                        value={ window.location.href.split('/').pop() }
                        onChange={ e => goToSelectedPage(e.target.value) }
                >
                    <option value="home" selected>My Profile & Account Details</option>
                    {
                        (
                            props.authUser.role === 0 &&
                            <option value="browse-classrooms">Browse for all classrooms</option>
                        ) ||
                        <option value="manage-classrooms">Manage my classrooms</option>
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