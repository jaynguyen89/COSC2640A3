import React from 'react';
import {connect} from 'react-redux';
import moment from 'moment';
import Spinner from "../../../shared/Spinner";
import Alert from "../../../shared/Alert";
import {IScheduleList} from "../redux/interfaces";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser
});

const mapActionsToProps = {};

const ClassName = (props: IScheduleList) => {
    return (
        <div className='row'>
            <div className='col s12'>
                <p className='title section-header'>
                    <i className="fas fa-angle-right" />
                    &nbsp;Schedules progress
                </p>
                { props.shouldShowSpinner && <Spinner /> }
                <Alert { ...props.statusMessage } closeAlert={ props.closeStatusMessage } />

                <table className='striped'>
                    <thead>
                        <tr>
                            <td>File ID</td>
                            <td>File Name</td>
                            <td>File Size</td>
                            <td>Uploaded On</td>
                            <td>Import Type</td>
                            <td>Status</td>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            (
                                props.schedules.length === 0 &&
                                <tr>
                                    <td colSpan={ 6 } className='center-align'>
                                        You have no schedule to import any data.
                                    </td>
                                </tr>
                            ) ||
                            props.schedules.map(schedule =>
                                <tr key={ schedule.id }>
                                    <td>{ schedule.fileId }</td>
                                    <td>{ schedule.fileName }</td>
                                    <td>{ schedule.fileSize } KB</td>
                                    <td>{ moment.unix(schedule.uploadedOn).format('DD MMM YYYY hh:mm') }</td>
                                    <td>{ (schedule.isForClassroom && 'Classrooms') || 'Students' }</td>
                                    <td>
                                        <span className={
                                            (schedule.status === 0 && 'purple-text') || (
                                                (schedule.status === 1 && 'blue-text') || (
                                                    (schedule.status === 2 && 'green-text') || (
                                                        (schedule.status === 3 && 'amber-text') || 'red-text'
                                                    )
                                                )
                                            )
                                        }>
                                            {
                                                schedule.status === 0 ? 'Awaiting' : (
                                                    schedule.status === 1 ? 'Processing' : (
                                                        schedule.status === 2 ? 'Done' : (
                                                            schedule.status === 3 ? 'Incomplete' : 'Failed'
                                                        )
                                                    )
                                                )
                                            }
                                        </span>
                                    </td>
                                </tr>
                            )
                        }
                    </tbody>
                </table>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ClassName);