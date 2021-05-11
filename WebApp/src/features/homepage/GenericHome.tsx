import React from 'react';
import {connect} from 'react-redux';

import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser
});

const mapActionsToProps = {};

const GenericHome = (props: any) => {console.log(props.authUser)
    return (
        <></>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(GenericHome);