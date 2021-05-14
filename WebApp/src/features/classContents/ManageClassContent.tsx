import React from 'react';
import {connect} from 'react-redux';

import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser
});

const mapActionsToProps = {};

const ManageClassContent = (props: any) => {
    return (
        <></>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ManageClassContent);