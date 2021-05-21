import React from 'react';
import {connect} from 'react-redux';
import _ from 'lodash';
import {defaultClassroomContent , IClassroomContentModal, IClassContent} from "../redux/interfaces";
import {
    DurationUnits,
    EMPTY_STRING,
    normalizeDt,
    TASK_CREATE,
    TASK_UPDATE,
    TASK_VIEW
} from "../../../providers/helpers";
import Spinner from "../../../shared/Spinner";
import Alert from "../../../shared/Alert";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser
});

const mapActionsToProps = {};

const ClassroomContentModal = (props: IClassroomContentModal) => {
    const [classroomcontent, setClassroomContent] = React.useState(defaultClassroomContent);

    React.useEffect(() => {
        setClassroomContent(props.selectedClassroomContent);
    }, [props.selectedClassroomContent]);

    return (
        <div className='modal' id='classroomModal'>
            <div className='row'>
                <div className='col s12'>
                    <p className='section-header'>
                        <i className="fas fa-school" />&nbsp;&nbsp;
                        {
                            (props.task === TASK_VIEW && 'Classroom Details') || (
                                (props.task === TASK_UPDATE && 'Update Classroom') || 'Add New Classroom'
                            )
                        }
                    </p>
                </div>
             </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ClassroomContentModal);