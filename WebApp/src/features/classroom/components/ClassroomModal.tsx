import React from 'react';
import {connect} from 'react-redux';
import _ from 'lodash';
import {defaultClassroom, defaultClassroomDetail, IClassroom, IClassroomModal} from "../redux/interfaces";
import {DurationUnits, EMPTY_STRING, TASK_CREATE, TASK_UPDATE, TASK_VIEW} from "../../../providers/helpers";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser
});

const mapActionsToProps = {};

const ClassroomModal = (props: IClassroomModal) => {
    const [classroom, setClassroom] = React.useState(defaultClassroom);

    React.useEffect(() => {

    }, [props.selectedClassroomId]);

    const updateClassroom = (field: string, value: string) => {
        if (field === 'className') setClassroom({ ...classroom, className: value } as IClassroom);
        if (field === 'price') setClassroom({ ...classroom, price: (value && Number(value)) || 0 } as IClassroom);
        if (field === 'capacity') setClassroom({ ...classroom, classroomDetail: { ...classroom.classroomDetail, capacity: (value && Number(value)) || 0 } } as IClassroom);
        if (field === 'commenceDate') setClassroom({ ...classroom, classroomDetail: { ...classroom.classroomDetail, commencedOn: value } } as IClassroom);
        if (field === 'duration') setClassroom({ ...classroom, classroomDetail: { ...classroom.classroomDetail, duration: (value && Number(value)) || 0 } } as IClassroom);
        if (field === 'durationUnit') setClassroom({ ...classroom, classroomDetail: { ...classroom.classroomDetail, durationUnit: Number(value) } } as IClassroom);
    }

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

                <div className='input-field col m6 s12'>
                    <i className='material-icons prefix'>account_circle</i>
                    <input id='teacherName' type='text' value={ classroom.teacherName || (localStorage.getItem('preferredName') as string) } readOnly />
                    <label htmlFor='teacherName'>Teacher Name</label>
                </div>

                <div className='input-field col m6 s12'>
                    <i className='material-icons prefix'>account_circle</i>
                    <input id='className' type='text' className='validate'
                           value={ classroom.className || EMPTY_STRING }
                           onChange={ e => updateClassroom('className', e.target.value) }
                    />
                    <label htmlFor='className'>Classroom Name</label>
                </div>

                <div className='input-field col m4 s12'>
                    <i className='material-icons prefix'>account_circle</i>
                    <input id='price' min='0' max='9999.99' type='number' className='validate' step='0.01'
                           value={ classroom.price || EMPTY_STRING }
                           onChange={ e => updateClassroom('price', e.target.value) }
                    />
                    <label htmlFor='price'>Price</label>
                </div>

                <div className='input-field col m4 s12'>
                    <i className='material-icons prefix'>account_circle</i>
                    <input id='capacity' min='1' max='32767' type='number' className='validate'
                           value={ classroom.classroomDetail.capacity || EMPTY_STRING }
                           onChange={ e => updateClassroom('capacity', e.target.value) }
                    />
                    <label htmlFor='capacity'>Classroom Capacity</label>
                </div>

                <div className='input-field col m4 s12'>
                    <i className='material-icons prefix'>account_circle</i>
                    <input id='commenceDate' type='text' className='validate'
                           value={ classroom.classroomDetail.commencedOn || EMPTY_STRING }
                           onChange={ e => updateClassroom('commenceDate', e.target.value) }
                    />
                    <label htmlFor='commenceDate'>Commence Date</label>
                </div>

                <div className='input-field col m6 s12'>
                    <i className='material-icons prefix'>account_circle</i>
                    <input id='duration' type='number' step='1' min='1' max='255' className='validate'
                           value={ classroom.classroomDetail.duration || EMPTY_STRING }
                           onChange={ e => updateClassroom('duration', e.target.value) }
                    />
                    <label htmlFor='duration'>Duration</label>
                </div>

                <div className='input-field col m6 s12'>
                    <select className='browser-default'
                            value={ classroom.classroomDetail.durationUnit || 0 }
                            onChange={ e => updateClassroom('durationUnit', e.target.value) }
                    >
                        {
                            DurationUnits.map(unit =>
                                <option key={ unit.index } value={ unit.index }>{ unit.text }</option>
                            )
                        }
                    </select>
                </div>

                <div className='col s12'>
                    {
                        props.task === TASK_UPDATE &&
                        <button className='btn waves-effect waves-light'
                                onClick={ () => props.handleUpdateBtn(classroom) }
                        >
                            <i className="fas fa-edit" />
                            &nbsp; Update
                        </button>
                    }

                    {
                        props.task === TASK_CREATE &&
                        <button className='btn waves-effect waves-light'
                                onClick={ () => props.handleCreateBtn(classroom) }
                        >
                            <i className="fas fa-plus-circle" />
                            &nbsp; Add
                        </button>
                    }
                </div>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ClassroomModal);