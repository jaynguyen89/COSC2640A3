import React from 'react';
import {connect} from 'react-redux';

import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import * as accountConstants from "../homepage/redux/constants";
import HeaderNav from "../../shared/HeaderNav";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser
});

const mapActionsToProps = {};

const ManageClassrooms = (props: any) => {
    return (
        <div className='container' style={{ marginTop: '2.5em' }}>
            {/*{*/}
            {/*    (*/}
            {/*        props.getStudentDetail.action === accountConstants.GET_STUDENT_DETAIL_REQUEST_SENT ||*/}
            {/*        props.getTeacherDetail.action === accountConstants.GET_TEACHER_DETAIL_REQUEST_SENT*/}
            {/*    ) &&*/}
            {/*    <div style={{ paddingTop: '3em' }}>*/}
            {/*        <Spinner />*/}
            {/*    </div>*/}
            {/*}*/}
            {/*<Alert { ...statusMessage } />*/}

            <div className='row'>
                <div className='col s12'>
                    <HeaderNav
                        location='manage-classrooms'
                        greetingName={ localStorage.getItem('preferredName') as string }
                    />

                    <div className='card'>
                        <div className='card-content'>
                            <div className='row'>
                                <div className='rol s12'>
                                    <p className='card-title left'>
                                        <i className="fab fa-buromobelexperte" />
                                        &nbsp; Your classrooms
                                    </p>

                                    <button className='btn waves-effect waves-light right'>
                                        <i className="fas fa-plus-circle" />
                                        &nbsp; Add classroom
                                    </button>
                                </div>
                            </div>


                            <div className='row'>

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
)(ManageClassrooms);