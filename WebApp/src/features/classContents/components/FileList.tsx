import React from 'react';
import {connect} from 'react-redux';
import moment from 'moment';
import Spinner from "../../../shared/Spinner";
import Alert from "../../../shared/Alert";
import {IFileList} from "../redux/interfaces";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser
});

const mapActionsToProps = {};

const ClassName = (props: IFileList) => {
    return (
        <div className='row'>
            <div className='col s12'>
                <p className='title section-header'>
                    <i className="fas fa-angle-right" />
                    &nbsp;Files currently Uploaded
                </p>
                { props.shouldShowSpinner && <Spinner /> }

                <table className='striped'>
                    <thead>
                        <tr>
                            <td>File ID</td>
                            <td>File Name</td>
                            <td>File Size</td>
                            <td>Uploaded On</td>
                            <td>Extension</td>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            (
                                props.files.length === 0 &&
                                <tr>
                                    <td colSpan={ 6 } className='center-align'>
                                        You have files currently uploaded
                                    </td>
                                </tr>
                            ) ||
                            props.files.map(filelist =>
                                <tr key={ filelist.id }>
                                    <td>{ filelist.name }</td>
                                    <td>{ filelist.type }</td>
                                    <td>{ filelist.extension } KB</td>
                                    <td>{ moment.unix(filelist.uploadedOn).format('DD MMM YYYY hh:mm') }</td>
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